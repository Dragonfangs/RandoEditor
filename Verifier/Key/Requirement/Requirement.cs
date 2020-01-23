using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;

namespace Verifier.Key.Requirement
{
	public enum RequirementObjectType
	{
		Undefined = 0,
		Simple,
		Complex,
	}
	
	[JsonConverter(typeof(RequirementConverter))]
	public abstract class Requirement
	{
		public RequirementObjectType myObjectType = RequirementObjectType.Undefined;

		public abstract void ConnectKeys();

		public abstract bool Unlocked(Inventory anInventory);
	}

	public class BaseSpecifiedConcreteClassConverter : DefaultContractResolver
	{
		protected override JsonConverter ResolveContractConverter(Type objectType)
		{
			if (typeof(Requirement).IsAssignableFrom(objectType) && !objectType.IsAbstract)
				return null; // pretend TableSortRuleConvert is not specified (thus avoiding a stack overflow)
			return base.ResolveContractConverter(objectType);
		}
	}

	public class RequirementConverter : JsonConverter
	{
		static JsonSerializerSettings SpecifiedSubclassConversion = new JsonSerializerSettings() { ContractResolver = new BaseSpecifiedConcreteClassConverter() };

		public override bool CanConvert(Type objectType)
		{
			return (objectType == typeof(Requirement));
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			JObject jo = JObject.Load(reader);
			switch (jo["myObjectType"].Value<int>())
			{
				case 1:
					return JsonConvert.DeserializeObject<SimpleRequirement>(jo.ToString(), SpecifiedSubclassConversion);
				case 2:
					return JsonConvert.DeserializeObject<ComplexRequirement>(jo.ToString(), SpecifiedSubclassConversion);
				default:
					throw new Exception();
			}
			throw new NotImplementedException();
		}

		public override bool CanWrite
		{
			get { return false; }
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException(); // won't be called because CanWrite returns false
		}
	}
}
