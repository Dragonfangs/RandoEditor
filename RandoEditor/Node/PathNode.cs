using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RandoEditor.Key;
using RandoEditor.Utils;
using RandoEditor.Key.Requirement;

namespace RandoEditor.Node
{
	[Serializable]
	public class PathNode
	{
		public PathNode()
		{
			id = Guid.NewGuid();
		}

		public Guid id;

		public NodeType myNodeType;

		public Vector2 myPos;

		public List<Guid> myConnectionIds = new List<Guid>();
		[NonSerialized]
		public List<PathNode> myConnections = new List<PathNode>();

		public void SetNodeType(NodeType aNodeType)
		{
			if (myNodeType == aNodeType)
				return;

			if (myNodeType == NodeType.Lock)
			{
				myRequirement.Clear();
			}

			if (myNodeType == NodeType.EventKey)
			{
				myEventKeyId = null;
			}

			myNodeType = aNodeType;
		}

		public void FormConnections(List<PathNode> nodes)
		{
			myConnections.Clear();
			foreach(var id in myConnectionIds)
			{
				var connection = nodes.FirstOrDefault(node => node.id == id);
				if(connection != null)
				{
					myConnections.Add(connection);
				}
			}
		}

		public void CreateConnection(PathNode otherNode)
		{
			if (!myConnectionIds.Contains(otherNode.id))
			{
				myConnectionIds.Add(otherNode.id);
				myConnections.Add(otherNode);
			}
		}

		public void RemoveConnection(PathNode otherNode)
		{
			if (myConnectionIds.Contains(otherNode.id))
			{
				myConnectionIds.Remove(otherNode.id);
				myConnections.Remove(otherNode);
			}
		}

		public Guid? myEventKeyId;
		[NonSerialized]
		public BaseKey myEventKey;

		public void SetEventKey(BaseKey key)
		{
			myEventKey = key;

			myEventKeyId = key?.Id;
		}

		public void ConnectKeys()
		{
			if (myEventKeyId.HasValue)
			{
				myEventKey = KeyManager.GetKey(myEventKeyId.Value);
			}

			myRequirement.ConnectKeys();
		}

		public ComplexRequirement myRequirement = new ComplexRequirement();
	}
}
