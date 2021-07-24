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

        public override string GetKeyName()
        {
            return KeyManager.GetMappedRandomKeyName(myRandomKeyIdentifier);
        }

        public void SetOriginalKey(BaseKey key)
        {
            myOriginalKeyId = key?.Id;
        }

        public BaseKey GetOriginalKey()
        {
            if (myOriginalKeyId.HasValue)
            {
                return KeyManager.GetKey(myOriginalKeyId.Value);
            }

            return null;
        }

        public string myRandomKeyIdentifier;
        public Guid? myOriginalKeyId;

        public override string Name()
        {
            return myRandomKeyIdentifier;
        }
    }
}
