using Verifier.Key.Requirement;
using System;

namespace Verifier.Key
{
	[Serializable]
	public class ComplexKey : BaseKey
	{
		public ComplexKey(Guid id, string name)
			:base(id, name)
		{

		}

		public ComplexRequirement myRequirement = new ComplexRequirement();
	}
}
