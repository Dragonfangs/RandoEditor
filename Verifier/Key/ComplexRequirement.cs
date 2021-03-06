﻿using System.Collections.Generic;
using System.Linq;

namespace Verifier.Key.Requirement
{
	public enum RequirementType
	{
		AND = 0,
		OR,
	}

	public static class RequirementTypeConverter
	{
		public static string Convert(RequirementType type)
		{
			switch (type)
			{
				case RequirementType.AND:
					return "All of";
				case RequirementType.OR:
					return "Any of";
			}

			return string.Empty;
		}

		public static RequirementType Convert(string type)
		{
			switch (type)
			{
				case "All of":
					return RequirementType.AND;
				case "Any of":
					return RequirementType.OR;
			}

			// Some default value
			return RequirementType.AND;
		}
	}

	public class ComplexRequirement : Requirement
	{
		public ComplexRequirement()
		{
			myObjectType = RequirementObjectType.Complex;
			myType = RequirementType.AND;
		}

		public ComplexRequirement(RequirementType type)
		{
			myObjectType = RequirementObjectType.Complex;
			myType = type;
		}

		public override void ConnectKeys()
		{
			myRequirements.ForEach(r => r.ConnectKeys());
		}

		public override bool Unlocked(Inventory anInventory)
		{ 	
			if(myType == RequirementType.AND)
			{
				return myRequirements.All(r => r.Unlocked(anInventory));
			}
			else if(myType == RequirementType.OR)
			{
				return myRequirements.Any(r => r.Unlocked(anInventory));
			}

			return false;
		}

		public void Clear()
		{
			myType = RequirementType.AND;
			myRequirements.Clear();
		}

		public RequirementType myType;
		public List<Requirement> myRequirements = new List<Requirement>();
	}
}
