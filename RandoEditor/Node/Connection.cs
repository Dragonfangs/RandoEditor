using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandoEditor.Node
{
	public class Connection : IEquatable<Connection>
	{
		public Connection(PathNode aNode, PathNode anotherNode)
		{
			node1 = aNode;
			node2 = anotherNode;
		}

		public bool Equals(Connection other)
		{
			return ((node1 == other.node1 && node2 == other.node2) ||
					(node1 == other.node2 && node2 == other.node1));
		}

		public override int GetHashCode()
		{
			int node1Hash = node1.GetHashCode();
			int node2Hash = node2.GetHashCode();

			return node1Hash ^ node2Hash;
		}

		public PathNode node1;
		public PathNode node2;
	}
}
