using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Key.Requirement
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

		public void Clear()
		{
			myType = RequirementType.AND;
			myRequirements.Clear();
		}

		public RequirementType myType;
		public List<Requirement> myRequirements = new List<Requirement>();
	}
}
