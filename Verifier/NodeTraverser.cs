using System;
using System.Collections.Generic;
using System.Linq;
using Verifier.Key;
using Verifier.Node;
using Verifier.SaveData;

namespace Verifier
{
	public class NodeTraverser
	{
		public bool VerifyBeatable()
		{
			var nodes = SaveManager.Data.Nodes;
			foreach (var node in nodes)
			{
				node.FormConnections(nodes);
				node.ConnectKeys();
			}

			var keyNodes = nodes.Where(node => node.myKey != null).ToList();
			var eventNodes = keyNodes.Where(node => node.myNodeType == NodeType.EventKey);
			var startNode = eventNodes.FirstOrDefault(x => String.Equals(x.myKey.Name, "Game Start", StringComparison.InvariantCultureIgnoreCase));
			var endNode = eventNodes.FirstOrDefault(x => String.Equals(x.myKey.Name, "Game Finish", StringComparison.InvariantCultureIgnoreCase));

			if (startNode == null || endNode == null)
			{
				return false;
			}

			var inventory = new Inventory();

			return SearchBeatable(startNode, endNode, keyNodes, inventory);
		}

		private bool SearchBeatable(PathNode startNode, PathNode endNode, List<PathNode> keyNodes, Inventory anInventory)
		{
			while (true)
			{
				if (PathExists(startNode, endNode, anInventory))
					return true;

				var reachableKeys = keyNodes.Where(node => !anInventory.myKeys.Contains(node.myKey)).Where(node => PathExists(startNode, node, anInventory)).ToList();

				var retracableKeys = reachableKeys.Where(node => PathExists(node, startNode, anInventory.Expand(node.myKey))).Select(node => node.myKey).ToList();

				if (retracableKeys.Any())
				{
					anInventory.myKeys.AddRange(retracableKeys);
				}
				else
				{
					return reachableKeys.Any(node => SearchBeatable(node, endNode, keyNodes, new Inventory(anInventory)));
				}
			}
		}

		private bool PathExists(PathNode currentNode, PathNode endNode, Inventory anInventory, List<PathNode> visitedNodes = null)
		{
			if (currentNode == endNode)
				return true;

			if (visitedNodes == null)
				visitedNodes = new List<PathNode>();
			
			if (visitedNodes.Contains(currentNode))
				return false;
			
			if (currentNode.myNodeType == NodeType.Lock && !currentNode.myRequirement.Unlocked(anInventory))
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
