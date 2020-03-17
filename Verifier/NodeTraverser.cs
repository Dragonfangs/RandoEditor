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

		public string GetWaveLog()
		{
			return myWaveLog.Print();
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

			var keyNodes = nodeCollection.myNodes.Where(node => node is KeyNode).ToList();
			var eventNodes = keyNodes.Where(node => node is EventKeyNode).Select(node => node as EventKeyNode);
			var startNode = eventNodes.FirstOrDefault(x => string.Equals(x.GetKey().Name, "Game Start", StringComparison.InvariantCultureIgnoreCase));
			var endNode = eventNodes.FirstOrDefault(x => string.Equals(x.GetKey().Name, "Game Finish", StringComparison.InvariantCultureIgnoreCase));

			if (startNode == null || endNode == null)
			{
				return (false, new WaveLog());
			}

			aStartInventory = aStartInventory ?? new Inventory();

			return SearchBeatable(startNode, endNode, keyNodes, aStartInventory, new WaveLog()).Result;
		}

		private async Task<(bool, WaveLog)> SearchBeatable(NodeBase startNode, NodeBase endNode, List<NodeBase> keyNodes, Inventory anInventory, WaveLog log)
		{
			while (true)
			{
				if (VerifyGoal(endNode, keyNodes, anInventory))
					return (true, log);

				var reachableKeys = keyNodes.AsParallel().Where(node => !anInventory.myNodes.Contains(node)).Where(node => PathExists(startNode, node, anInventory)).ToList();

				var retracableKeys = reachableKeys.AsParallel().Where(node => PathExists(node, startNode, anInventory.Expand(node))).ToList();

				if (retracableKeys.Any())
				{
					log.AddLive(new List<NodeBase>(retracableKeys));
					anInventory.myNodes.AddRange(retracableKeys);
				}
				else
				{
					var redundantNodes = new List<NodeBase>();
					for(int i=0;i<reachableKeys.Count();i++)
					{
						for (int j = i + 1; j < reachableKeys.Count; j++)
						{
							if(PathExists(reachableKeys[i], reachableKeys[j], anInventory) && PathExists(reachableKeys[j], reachableKeys[i], anInventory))
							{
								redundantNodes.Add(reachableKeys[j]);
							}
						}
					}

					redundantNodes = redundantNodes.Distinct().ToList();
					reachableKeys.RemoveAll(node => redundantNodes.Contains(node));

					var searchResults = await Task.WhenAll(reachableKeys.Select(node => Task.Run(async () =>
					{
						return (await SearchBeatable(node, endNode, keyNodes, new Inventory(anInventory), new WaveLog()));
					})));

					foreach (var result in searchResults)
					{
						if(result.Item1)
						{
							log.AddLive(result.Item2);
							log.ClearDead();
							return (true, log);
						}
						else
						{
							log.AddDead(result.Item2);
						}
					}

					return (false, log);
				}
			}
		}

		private bool VerifyGoal(NodeBase endNode, List<NodeBase> keyNodes, Inventory anInventory)
		{
			if (fullComplete)
			{
				return keyNodes.All(key => anInventory.myNodes.Contains(key));
			}
			else
			{
				return anInventory.myNodes.Contains(endNode);
			}
		}

		private bool PathExists(NodeBase currentNode, NodeBase endNode, Inventory anInventory, List<NodeBase> visitedNodes = null)
		{
			if (currentNode == endNode)
				return true;

			if (visitedNodes == null)
				visitedNodes = new List<NodeBase>();
			
			if (visitedNodes.Contains(currentNode))
				return false;
			
			if (currentNode is LockNode lockNode && !anInventory.Unlocks(lockNode.myRequirement))
				return false;

			visitedNodes.Add(currentNode);
			foreach(var connection in currentNode.myConnections)
			{
				if (PathExists(connection, endNode, anInventory, visitedNodes))
					return true;
			}

			return false;
		}
	}
}
