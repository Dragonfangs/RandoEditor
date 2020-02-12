using System;
using Common.Memento;

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

		public override RequirementMemento CreateMemento()
		{
			var memento = new SimpleRequirementMemento();

			memento.myRepeatCount = myRepeatCount;
			memento.myKeyId = myKeyId;

			return memento;
		}

		public override void RestoreMemento(RequirementMemento memento)
		{
			if (memento is SimpleRequirementMemento simpleMemento)
			{
				myRepeatCount = simpleMemento.myRepeatCount;
				myKeyId = simpleMemento.myKeyId;
			}
		}

		public uint myRepeatCount = 1;

		public Guid myKeyId;

		public BaseKey GetKey()
		{
			return KeyManager.GetKey(myKeyId);
		}
	}
}
