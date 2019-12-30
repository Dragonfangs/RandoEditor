using RandoEditor.Key.Requirement;
using System;

namespace RandoEditor.Key
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
