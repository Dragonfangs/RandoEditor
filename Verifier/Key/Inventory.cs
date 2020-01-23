using System.Collections.Generic;

namespace Verifier.Key
{
	public class Inventory
	{
		public Inventory()
		{
			myKeys = new List<BaseKey>();
		}

		public Inventory(Inventory other)
		{
			myKeys = new List<BaseKey>(other.myKeys);
		}

		public Inventory Expand(BaseKey key)
		{
			var newInv = new Inventory(this);
			newInv.myKeys.Add(key);
			return newInv;
		}

		public List<BaseKey> myKeys;
	}
}
