using Common.Memento;
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

		public override bool ContainsKey(Guid id)
		{
			return myRequirements.Any(req => req.ContainsKey(id));
		}

        public override bool ContainsKeyWithString(string searchString)
        {
            return myRequirements.Any(req => req.ContainsKeyWithString(searchString));
        }

        // Returns true if Requirement became obsolete because of the removal
        public override bool RemoveKey(Guid id)
		{
			if (!myRequirements.Any())
				return false;

			var reqsToRemove = new List<Requirement>();
			foreach (var req in myRequirements)
			{
				if(req.RemoveKey(id))
				{
					// TODO: Somehow collapse a Complex Req that has been reduced to containing a single other req to just be the contained req instead
					reqsToRemove.Add(req);
				}
			}

			myRequirements.RemoveAll(req => reqsToRemove.Contains(req));

			if (myRequirements.Any())
				return false;

			return true;
		}

		public override void ReplaceKey(Guid id, Guid otherId)
		{
			foreach (var req in myRequirements)
			{
				req.ReplaceKey(id, otherId);
			}
		}

		public override RequirementMemento CreateMemento()
		{
			var memento = new ComplexRequirementMemento();

			memento.myType = myType;
			memento.myRequirements = myRequirements.Select(x => x.CreateMemento()).ToList();

			return memento;
		}

		public override void RestoreMemento(RequirementMemento memento)
		{
			if(memento is ComplexRequirementMemento complexMemento)
			{
				myType = complexMemento.myType;

				myRequirements = complexMemento.myRequirements.Select(x => CreateFromMemento(x)).ToList();
			}
		}

		public RequirementType myType;
		public List<Requirement> myRequirements = new List<Requirement>();
	}
}
