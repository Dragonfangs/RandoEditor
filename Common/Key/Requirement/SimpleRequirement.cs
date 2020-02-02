using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Key.Requirement
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
		}

		public SimpleRequirement(BaseKey key)
			:this(key.Id)
		{
		}

		public uint myRepeatCount = 1;

		public Guid myKeyId;

		public BaseKey GetKey()
		{
			return KeyManager.GetKey(myKeyId);
		}
	}
}
