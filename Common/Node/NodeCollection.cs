
using Common.Memento;
using Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Node
{
	public class NodeCollection
	{
		public List<NodeBase> myNodes = new List<NodeBase>();

		public void InitializeNodes(SaveData.SaveData someData)
		{
			myNodes = someData.Nodes;
			foreach (var node in myNodes)
			{
				node.FormConnections(this);
			}
		}

		public NodeMemento AddNode(NodeBase newNode)
		{
			myNodes.Add(newNode);

			return new NodeCreatedMemento(newNode.id);
		}

		public NodeMemento RemoveNode(NodeBase nodeToDelete)
		{
			var memento = new NodeDeletedMemento(nodeToDelete);
			foreach (var node in myNodes)
			{
				if (node.myConnections.Contains(nodeToDelete))
				{
					memento.nodesWithConnectionToThis.Add(node.id);
					node.RemoveConnection(nodeToDelete);
				}
			}
			myNodes.Remove(nodeToDelete);

			return memento;
		}

		public NodeMemento CreateConnectionMemento(NodeBase node)
		{
			return CreateConnectionMemento(new List<NodeBase> { node });
		}

		public NodeMemento CreateConnectionMemento(List<NodeBase> nodes)
		{
			var memento = new NodeConnectionMemento();

			memento.nodeConnections = nodes.ToDictionary(node => node.id, node => new List<Guid>(node.myConnectionIds));

			return memento;
		}

		public void RestoreMemento(NodeMemento memento)
		{
			if(memento is NodePositionMemento positionMemento)
			{
				var node = myNodes.FirstOrDefault(x => x.id == positionMemento.nodeId);
				if(node != null)
				{
					node.RestoreMemento(positionMemento, this);
				}
			}
			else if (memento is NodeConnectionMemento connectionMemento)
			{
				foreach(var nodeMemento in connectionMemento.nodeConnections)
				{
					var node = myNodes.FirstOrDefault(x => x.id == nodeMemento.Key);
					if (node != null)
					{
						node.myConnectionIds = nodeMemento.Value;
						node.FormConnections(this);
					}
				}
			}
			else if (memento is NodeCreatedMemento createdMemento)
			{
				var node = myNodes.FirstOrDefault(x => x.id == createdMemento.nodeId);
				if (node != null)
				{
					RemoveNode(node);
				}
			}
			else if (memento is NodeDeletedMemento deletedMemento)
			{
				AddNode(deletedMemento.node);
				foreach (var node in myNodes)
				{
					if (deletedMemento.nodesWithConnectionToThis.Contains(node.id))
					{
						node.CreateConnection(deletedMemento.node);
					}
				}
			}
		}
	}
}
