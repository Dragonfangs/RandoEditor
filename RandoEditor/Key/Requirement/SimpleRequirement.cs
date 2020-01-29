using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandoEditor.Key.Requirement
{
	public class SimpleRequirement : Requirement
	{
		public SimpleRequirement()
		{
			myObjectType = RequirementObjectType.Simple;
		}

		public SimpleRequirement(Guid keyId)
			:this()
		{
			myKeyId = keyId;
			ConnectKeys();
		}

		public SimpleRequirement(BaseKey key)
			:this(key.Id)
		{
		}

		public uint myRepeatCount = 1;

		public Guid myKeyId;
		[NonSerialized]
		public BaseKey myKey;

		public override void ConnectKeys()
		{
			myKey = KeyManager.GetKey(myKeyId);
		}
	}
}
