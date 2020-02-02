using Common.Key;
using System;

namespace Common.Node
{
	[Serializable]
	public class RandomKeyNode : KeyNode
	{
		public RandomKeyNode()
		{
			id = Guid.NewGuid();

			myNodeType = NodeType.RandomKey;
		}

		public override BaseKey GetKey()
		{
			return KeyManager.GetMappedRandomKey(myRandomKeyIdentifier);
		}

		public string myRandomKeyIdentifier;
	}
}
