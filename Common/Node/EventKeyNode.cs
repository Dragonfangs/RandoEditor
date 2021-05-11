using System;
using Common.Key;

namespace Common.Node
{
	[Serializable]
	public class EventKeyNode : KeyNode
	{
		public EventKeyNode()
		{
			id = Guid.NewGuid();

			myNodeType = NodeType.EventKey;
		}
		
		public Guid? myKeyId;

		public void SetKey(BaseKey key)
		{
			myKeyId = key?.Id;
		}

		public override BaseKey GetKey()
		{
			if (myKeyId.HasValue)
			{
				return KeyManager.GetKey(myKeyId.Value);
			}

			return null;
		}

        public override string GetKeyName()
        {
            return Name();
        }

        public override string Name()
        {
            if(!myKeyId.HasValue)
            {
                return "Empty Event";
            }

            var key = KeyManager.GetKey(myKeyId.Value);
            if(key == null)
            {
                return "Empty Event";
            }

            return key.Name;
        }
    }
}
