using System.Collections.Generic;
using Verifier.Node;

namespace Verifier.Key
{
	public class Inventory
	{
		public Inventory()
		{
			myNodes = new List<PathNode>();
		}

		public Inventory(Inventory other)
		{
			myNodes = new List<PathNode>(other.myNodes);
		}

		public Inventory Expand(PathNode key)
		{
			var newInv = new Inventory(this);
			newInv.myNodes.Add(key);
			return newInv;
		}

		public List<PathNode> myNodes;
	}
}
