using System;
using System.Collections.Generic;
using System.Linq;

using Verifier.Key;
using Verifier.Key.Requirement;

namespace Verifier.Node
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

		public Guid? myEventKeyId;
		[NonSerialized]
		public BaseKey myKey;

		public void SetEventKey(BaseKey key)
		{
			myKey = key;

			myEventKeyId = key?.Id;
		}

		public string myRandomKeyIdentifier;

		public void ConnectKeys()
		{
			if (myEventKeyId.HasValue)
			{
				myKey = KeyManager.GetKey(myEventKeyId.Value);
			}
			else if (!String.IsNullOrEmpty(myRandomKeyIdentifier))
			{
				myKey = KeyManager.GetMappedRandomKey(myRandomKeyIdentifier);
			}

			myRequirement.ConnectKeys();
		}

		public ComplexRequirement myRequirement = new ComplexRequirement();
	}
}
