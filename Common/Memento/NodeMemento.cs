using Common.Node;
using Common.Utils;
using System;
using System.Collections.Generic;

namespace Common.Memento
{
	public class NodeMemento : Memento
	{

	}

	public class NodePositionMemento : NodeMemento
	{
		public Guid nodeId;
		public Vector2 nodePos;
	}

	public class NodeConnectionMemento : NodeMemento
	{
		public Dictionary<Guid, List<Guid>> nodeConnections = new Dictionary<Guid, List<Guid>>();
	}

	public class NodeCreatedMemento : NodeMemento
	{
		public NodeCreatedMemento()
		{
		}

		public NodeCreatedMemento(Guid id)
		{
			nodeId = id;
		}

		public Guid nodeId;
	}

	public class NodeDeletedMemento : NodeMemento
	{
		public NodeDeletedMemento()
		{
		}

		public NodeDeletedMemento(NodeBase deletedNode)
		{
			node = deletedNode;
		}

		public NodeBase node;
		public List<Guid> nodesWithConnectionToThis = new List<Guid>();
	}
}
