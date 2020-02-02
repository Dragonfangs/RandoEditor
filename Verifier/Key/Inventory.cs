using Common.Key.Requirement;
using Common.Node;
using System.Collections.Generic;
using System.Linq;

namespace Verifier.Key
{
	public class Inventory
	{
		public Inventory()
		{
			myNodes = new List<NodeBase>();
		}

		public Inventory(Inventory other)
		{
			myNodes = new List<NodeBase>(other.myNodes);
		}

		public Inventory Expand(NodeBase key)
		{
			var newInv = new Inventory(this);
			newInv.myNodes.Add(key);
			return newInv;
		}

		public bool Unlocks(Requirement req)
		{
			if (req is ComplexRequirement complex)
			{
				if (complex.myType == RequirementType.AND)
				{
					return complex.myRequirements.All(r => Unlocks(r));
				}
				else if (complex.myType == RequirementType.OR)
				{
					return complex.myRequirements.Any(r => Unlocks(r));
				}
			}

			if(req is SimpleRequirement simple)
			{
				return myNodes.Count(node => (node as KeyNode).GetKey() == simple.GetKey()) >= simple.myRepeatCount;
			}

			return false;
		}

		public List<NodeBase> myNodes;
	}
}
