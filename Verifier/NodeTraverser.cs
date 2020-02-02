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
		public bool VerifyBeatable(Common.SaveData.SaveData someData, Dictionary<string, Guid> aRandomMap)
		{
			KeyManager.Initialize(someData);
			KeyManager.SetRandomKeyMap(aRandomMap);

			var nodes = someData.Nodes;
			foreach (var node in nodes)
			{
				node.FormConnections(nodes);
			}

			var keyNodes = nodes.Where(node => node is KeyNode).ToList();
			var eventNodes = keyNodes.Where(node => node is EventKeyNode).Select(node => node as EventKeyNode);
			var startNode = eventNodes.FirstOrDefault(x => string.Equals(x.GetKey().Name, "Game Start", StringComparison.InvariantCultureIgnoreCase));
			var endNode = eventNodes.FirstOrDefault(x => string.Equals(x.GetKey().Name, "Game Finish", StringComparison.InvariantCultureIgnoreCase));

			if (startNode == null || endNode == null)
			{
				return false;
			}

			var inventory = new Inventory();

			return SearchBeatable(startNode, endNode, keyNodes, inventory);
		}

		private bool SearchBeatable(NodeBase startNode, NodeBase endNode, List<NodeBase> keyNodes, Inventory anInventory)
		{
			while (true)
			{
				if (PathExists(startNode, endNode, anInventory))
					return true;

				var reachableKeys = keyNodes.Where(node => !anInventory.myNodes.Contains(node)).Where(node => PathExists(startNode, node, anInventory)).ToList();

				var retracableKeys = reachableKeys.Where(node => PathExists(node, startNode, anInventory.Expand(node))).ToList();

				if (retracableKeys.Any())
				{
					anInventory.myNodes.AddRange(retracableKeys);
				}
				else
				{
					return reachableKeys.Any(node => SearchBeatable(node, endNode, keyNodes, new Inventory(anInventory)));
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
