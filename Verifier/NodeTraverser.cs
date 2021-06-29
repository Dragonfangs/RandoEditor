using Common.Key;
using Common.Log;
using Common.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using Verifier.Key;

namespace Verifier
{
	public class NodeTraverser
	{
		private bool fullComplete;
		private WaveLog myWaveLog;
		private List<NodeBase> globalReachable = new List<NodeBase>();

		public string GetWaveLog()
		{
			return myWaveLog.Print();
		}

        public LogLayer DetailedLog { get; private set; }

        public List<NodeBase> GetUnreachable()
		{
			return globalReachable;
		}

		public bool VerifyBeatable(Common.SaveData.SaveData someData, Dictionary<string, Guid> aRandomMap, Inventory aStartInventory = null)
		{
			fullComplete = false;

			var result = SearchBeatableStart(someData, aRandomMap, aStartInventory);
			myWaveLog = result.Item2;
			return result.Item1;
		}

		public bool VerifyFullCompletable(Common.SaveData.SaveData someData, Dictionary<string, Guid> aRandomMap, Inventory aStartInventory = null)
		{
			fullComplete = true;
			var result = SearchBeatableStart(someData, aRandomMap, aStartInventory);
			myWaveLog = result.Item2;
			return result.Item1;
		}

		private (bool, WaveLog) SearchBeatableStart(Common.SaveData.SaveData someData, Dictionary<string, Guid> aRandomMap, Inventory aStartInventory)
		{
            DetailedLog = new LogLayer("Verification");

			KeyManager.Initialize(someData);
			KeyManager.SetRandomKeyMap(aRandomMap);

			var nodeCollection = new NodeCollection();
			nodeCollection.InitializeNodes(someData);

			var keyNodes = nodeCollection.myNodes.Where(node => node is KeyNode && (node as KeyNode).GetKey() != null).ToList();
			var eventNodes = keyNodes.Where(node => node is EventKeyNode).Select(node => node as EventKeyNode);
			var startNode = eventNodes.FirstOrDefault(x => string.Equals(x.GetKey().Name, "Game Start", StringComparison.InvariantCultureIgnoreCase));
			var endNode = eventNodes.FirstOrDefault(x => string.Equals(x.GetKey().Name, "Game Finish", StringComparison.InvariantCultureIgnoreCase));

            DetailedLog.AddChild("keyNodes", keyNodes.Select(key => key.Name()));
            DetailedLog.AddChild("eventNodes", eventNodes.Select(key => key.Name()));

            if (startNode == null || endNode == null)
			{
				return (false, new WaveLog());
			}

			aStartInventory = aStartInventory ?? new Inventory();

			var result = SearchBeatable(startNode, endNode, keyNodes, aStartInventory, DetailedLog);

            DetailedLog.AddChild("Search complete");

            globalReachable = keyNodes.Except(globalReachable).ToList();

            DetailedLog.AddChild("Unreachable nodes", globalReachable.Select(key => GetNodeWithKeyName(key)));

			return (result.Item1 != null, result.Item2);
		}

		private (Inventory, WaveLog) SearchBeatable(NodeBase startNode, NodeBase endNode, List<NodeBase> keyNodes, Inventory anInventory, LogLayer detailedLog)
		{
			return SearchBeatable(null, startNode, endNode, keyNodes, anInventory, detailedLog);
		}

		private (Inventory, WaveLog) SearchBeatable(NodeBase baseNode, NodeBase startNode, NodeBase endNode, List<NodeBase> keyNodes, Inventory anInventory, LogLayer detailedLog)
		{
            var localLog = detailedLog.AddChild("Search");

            localLog.AddChild($"Start Node: {startNode.Name()}");
            localLog.AddChild($"End Node: {endNode.Name()}");

            var log = new WaveLog();
			
			var currentSearcher = new FillSearcher();

			var reachableKeys = new List<NodeBase>();

            var stepCount = 1;
			while (true)
			{
                var stepLog = localLog.AddChild($"Step {stepCount++}");
                stepLog.AddChild(anInventory.GetKeyLog());

				reachableKeys.RemoveAll(node => anInventory.myNodes.Contains(node));
				reachableKeys.AddRange(currentSearcher.ContinueSearch(startNode, anInventory, node => (node is KeyNode) && !anInventory.myNodes.Contains(node)));

                stepLog.AddChild("Reachable Keys", reachableKeys.Select(key => GetNodeWithKeyName(key)));

                globalReachable = globalReachable.Union(reachableKeys).ToList();

                if (VerifyGoal(startNode, endNode, keyNodes, anInventory) || (baseNode != null && PathExists(startNode, baseNode, anInventory)))
                {
                    stepLog.AddChild("Goal verified");
                    return (anInventory, log);
                }

                var retracableKeys = reachableKeys.AsParallel().Where(node => node == endNode || PathExists(node, startNode, anInventory.Expand(node))).ToList();

                stepLog.AddChild("Retracable Keys", retracableKeys.Select(key => GetNodeWithKeyName(key)));

                if (retracableKeys.Any())
				{
					log.AddLive(new List<NodeBase>(retracableKeys));
					anInventory.myNodes.AddRange(retracableKeys);
				}
				else
				{
					var redundantNodes = new List<NodeBase>();
					for(int i = 0; i < reachableKeys.Count(); i++)
					{
						for (int j = 0; j < reachableKeys.Count; j++)
						{
							if(reachableKeys[i] != reachableKeys[j] &&
								!redundantNodes.Contains(reachableKeys[i]) &&
								!redundantNodes.Contains(reachableKeys[j]) &&
								PathExists(reachableKeys[i], reachableKeys[j], anInventory))
							{
								redundantNodes.Add(reachableKeys[j]);
							}
						}
					}

                    var relevantNodes = reachableKeys.Where(node => !redundantNodes.Contains(node));

                    if (redundantNodes.Any())
                    {
                        stepLog.AddChild("Redundant Keys", redundantNodes.Select(key => key.Name()));
                        stepLog.AddChild("Relevant Keys", relevantNodes.Select(key => key.Name()));
                    }

                    bool continuation = false;
					foreach (var key in relevantNodes)
					{
						var (newInv, newLog) = SearchBeatable(baseNode ?? startNode, key, endNode, keyNodes, new Inventory(anInventory), stepLog);
						if (newInv != null)
						{
                            stepLog.AddChild("Search succeeded");

                            log.AddLive(newLog);
							log.ClearDead();

							anInventory = newInv;
							
							continuation = true;

							break;
						}
						else
						{
							log.AddDead(newLog);
						}
					}

					if (!continuation)
					{
                        stepLog.AddChild("All Searches failed");
                        return (null, log);
					}
				}
			}
		}

		private bool VerifyGoal(NodeBase startNode, NodeBase endNode, List<NodeBase> keyNodes, Inventory anInventory)
		{
			if (fullComplete)
			{
				var nodesWithItem = keyNodes.Where(node => node is RandomKeyNode).Select(node => node as RandomKeyNode).Where(node => node.GetKey() != null);
				return nodesWithItem.All(node => anInventory.myNodes.Contains(node)) && PathExists(startNode, endNode, anInventory);
			}
			else
			{
                return PathExists(startNode, endNode, anInventory);
			}
		}

		public static bool PathExists(NodeBase startNode, NodeBase endNode, Inventory anInventory, List<NodeBase> visitedNodes = null)
		{
			if (startNode == endNode)
				return true;

			if (visitedNodes == null)
				visitedNodes = new List<NodeBase>();
			
			if (visitedNodes.Contains(startNode))
				return false;
			
			if (startNode is LockNode lockNode && !anInventory.Unlocks(lockNode.myRequirement))
				return false;

			visitedNodes.Add(startNode);
			foreach(var connection in startNode.myConnections)
			{
				if (PathExists(connection, endNode, anInventory, visitedNodes))
					return true;
			}

			return false;
		}

        private static string GetNodeWithKeyName(NodeBase keyNode)
        {
            if (keyNode is RandomKeyNode randomNode)
            {
                return $"{randomNode.Name()} - {randomNode.GetKeyName()}";
            }

            if (keyNode is EventKeyNode)
            {
                return $"{keyNode.Name()}";
            }

            return string.Empty;
        }
	}
}
