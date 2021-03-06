﻿using Common.Key;
using Common.Key.Requirement;
using Common.Log;
using Common.Node;
using System;
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
			myKeys = new List<BaseKey>(other.myKeys);
		}

		public IEnumerable<BaseKey> GetCombinedKeys()
		{
			return myNodes.Select(node => (node as KeyNode).GetKey()).Where(key => key != null).Concat(myKeys);
		}

		public bool ContainsKey(Guid keyId)
		{
			return GetCombinedKeys().Any(key => key.Id == keyId);
		}

        public LogLayer GetKeyLog()
        {
            return new LogLayer("Inventory", GetCombinedKeys().GroupBy(key => key.Id).Select(group => group.Count() > 1 ? $"{KeyManager.GetKeyName(group.Key)} - {group.Count()}" : KeyManager.GetKeyName(group.Key)));
        }

		public Inventory Expand(NodeBase key)
		{
			var newInv = new Inventory(this);
			newInv.myNodes.Add(key);
			return newInv;
		}

		public Inventory Expand(BaseKey key)
		{
			var newInv = new Inventory(this);
			newInv.myKeys.Add(key);
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
				var targetKey = simple.GetKey();
				if (targetKey is ComplexKey cKey)
				{
					return Unlocks(cKey.myRequirement);
				}
				else
				{
					var combinedKeys = GetCombinedKeys();

					if (simple.isInverted)
						return !combinedKeys.Any(key => key == targetKey);

					return combinedKeys.Count(key => key == targetKey) >= simple.myRepeatCount;
				}
			}

			return false;
		}

		public List<NodeBase> myNodes = new List<NodeBase>();
		public List<BaseKey> myKeys = new List<BaseKey>();
	}
}
