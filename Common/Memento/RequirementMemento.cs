using Common.Key.Requirement;
using System;
using System.Collections.Generic;

namespace Common.Memento
{
	public class RequirementMemento : Memento
	{

	}

	public class ComplexRequirementMemento : RequirementMemento
	{
		public RequirementType myType;
		public List<RequirementMemento> myRequirements = new List<RequirementMemento>();
	}

	public class SimpleRequirementMemento : RequirementMemento
	{
		public uint myRepeatCount = 1;
		public Guid myKeyId;
	}
}
