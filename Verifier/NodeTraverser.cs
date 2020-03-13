using Common.Key;
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
		private WaveLog myWaveLog = new WaveLog();

		public bool VerifyBeatable(Common.SaveData.SaveData someData, Dictionary<string, Guid> aRandomMap, Inventory aStartInventory = null)
		{
			fullComplete = false;
			return SearchBeatableStart(someData, aRandomMap, aStartInventory);
		}

		public bool VerifyFullCompletable(Common.SaveData.SaveData someData, Dictionary<string, Guid> aRandomMap, Inventory aStartInventory = null)
		{
			fullComplete = true;
			return SearchBeatableStart(someData, aRandomMap, aStartInventory);
		}

		public string GetWaveLog()
		{
			return myWaveLog.Print();
		}

		private bool SearchBeatableStart(Common.SaveData.SaveData someData, Dictionary<string, Guid> aRandomMap, Inventory aStartInventory)
		{
			myWaveLog.Clear();

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
				return false;
			}

			aStartInventory = aStartInventory ?? new Inventory();

			return SearchBeatable(startNode, endNode, keyNodes, aStartInventory, myWaveLog);
		}

		private bool SearchBeatable(NodeBase startNode, NodeBase endNode, List<NodeBase> keyNodes, Inventory anInventory, WaveLog log)
		{
			while (true)
			{
				if (VerifyGoal(startNode, endNode, keyNodes, anInventory, log))
					return true;

				var reachableKeys = keyNodes.Where(node => !anInventory.myNodes.Contains(node)).Where(node => PathExists(startNode, node, anInventory)).ToList();

				var retracableKeys = reachableKeys.Where(node => PathExists(node, startNode, anInventory.Expand(node))).ToList();

				if (retracableKeys.Any())
				{
					log.AddLive(new List<NodeBase>(retracableKeys));
					anInventory.myNodes.AddRange(retracableKeys);
				}
				else
				{
					foreach (var node in reachableKeys)
					{
						var deepLog = new WaveLog();
						if(SearchBeatable(node, endNode, keyNodes, new Inventory(anInventory), deepLog))
						{
							log.AddLive(deepLog);
							log.ClearDead();
							return true;
						}
						else
						{
							log.AddDead(deepLog);
						}
					}
					return false;
				}
			}
		}

		private bool VerifyGoal(NodeBase startNode, NodeBase endNode, List<NodeBase> keyNodes, Inventory anInventory, WaveLog log)
		{
			if (fullComplete)
			{
				return keyNodes.All(key => anInventory.myNodes.Contains(key));
			}
			else
			{
				if (PathExists(startNode, endNode, anInventory))
				{
					log.AddLive(new List<NodeBase> { endNode });
					return true;
				}
				else
				{
					return false;
				}
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
