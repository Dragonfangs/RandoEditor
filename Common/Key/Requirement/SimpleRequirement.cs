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

			// Only setting keys are allowed to be inverted
			if (!KeyManager.IsSetting(myKeyId))
				isInverted = false;
		}

		public SimpleRequirement(BaseKey key)
			:this(key.Id)
		{
		}

		public override RequirementMemento CreateMemento()
		{
			var memento = new SimpleRequirementMemento();

			memento.myRepeatCount = myRepeatCount;
			memento.isInverted = isInverted;
			memento.myKeyId = myKeyId;

			return memento;
		}

		public override void RestoreMemento(RequirementMemento memento)
		{
			if (memento is SimpleRequirementMemento simpleMemento)
			{
				myRepeatCount = simpleMemento.myRepeatCount;
				isInverted = simpleMemento.isInverted;
				myKeyId = simpleMemento.myKeyId;
			}
		}

		public override bool ContainsKey(Guid id)
		{
			return myKeyId == id;
		}

		public override bool RemoveKey(Guid id)
		{
			if (myKeyId != id)
				return false;

			myKeyId = Guid.Empty;
			return true;
		}

		public override void ReplaceKey(Guid id, Guid otherId)
		{
			if (myKeyId != id)
				return;

			myKeyId = otherId;

			// Only setting keys are allowed to be inverted
			if (!KeyManager.IsSetting(myKeyId))
				isInverted = false;
		}

		public uint myRepeatCount = 1;

		public bool isInverted = false;

		public Guid myKeyId;

		public BaseKey GetKey()
		{
			return KeyManager.GetKey(myKeyId);
		}
	}
}
