using Common.Key;
using Common.Node;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
			KeyManager.Initialize(someData);
			KeyManager.SetRandomKeyMap(aRandomMap);

			var nodeCollection = new NodeCollection();
			nodeCollection.InitializeNodes(someData);

			var keyNodes = nodeCollection.myNodes.Where(node => node is KeyNode && (node as KeyNode).GetKey() != null).ToList();
			var eventNodes = keyNodes.Where(node => node is EventKeyNode).Select(node => node as EventKeyNode);
			var startNode = eventNodes.FirstOrDefault(x => string.Equals(x.GetKey().Name, "Game Start", StringComparison.InvariantCultureIgnoreCase));
			var endNode = eventNodes.FirstOrDefault(x => string.Equals(x.GetKey().Name, "Game Finish", StringComparison.InvariantCultureIgnoreCase));

			if (startNode == null || endNode == null)
			{
				return (false, new WaveLog());
			}

			aStartInventory = aStartInventory ?? new Inventory();

			var result = SearchBeatable(startNode, endNode, keyNodes, aStartInventory);

			globalReachable = keyNodes.Except(globalReachable).ToList();

			return (result.Item1 != null, result.Item2);
		}

		private (Inventory, WaveLog) SearchBeatable(NodeBase startNode, NodeBase endNode, List<NodeBase> keyNodes, Inventory anInventory)
		{
			return SearchBeatable(null, startNode, endNode, keyNodes, anInventory);
		}

		private (Inventory, WaveLog) SearchBeatable(NodeBase baseNode, NodeBase startNode, NodeBase endNode, List<NodeBase> keyNodes, Inventory anInventory)
		{
			var log = new WaveLog();
			
			var currentSearcher = new FillSearcher();

			var reachableKeys = new List<NodeBase>();

			while (true)
			{
				if (VerifyGoal(endNode, keyNodes, anInventory) || (baseNode != null && PathExists(startNode, baseNode, anInventory)))
					return (anInventory, log);
				
				reachableKeys.RemoveAll(node => anInventory.myNodes.Contains(node));
				reachableKeys.AddRange(currentSearcher.ContinueSearch(startNode, anInventory, node => (node is KeyNode) && !anInventory.myNodes.Contains(node)));

				globalReachable = globalReachable.Union(reachableKeys).ToList();

				var retracableKeys = reachableKeys.AsParallel().Where(node => node == endNode || PathExists(node, startNode, anInventory.Expand(node))).ToList();

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

					bool continuation = false;
					foreach (var key in reachableKeys.Where(node => !redundantNodes.Contains(node)))
					{
						var (newInv, newLog) = SearchBeatable(baseNode ?? startNode, key, endNode, keyNodes, new Inventory(anInventory));
						if (newInv != null)
						{
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
						return (null, log);
					}
				}
			}
		}

		private bool VerifyGoal(NodeBase endNode, List<NodeBase> keyNodes, Inventory anInventory)
		{
			if (fullComplete)
			{
				var nodesWithItem = keyNodes.Where(node => node is RandomKeyNode).Select(node => node as RandomKeyNode).Where(node => node.GetKey() != null);
				return nodesWithItem.All(node => anInventory.myNodes.Contains(node)) && anInventory.myNodes.Contains(endNode);
			}
			else
			{
				return anInventory.myNodes.Contains(endNode);
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
	}
}
