using Common.Memento;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;

namespace Common.Key.Requirement
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
		public abstract RequirementMemento CreateMemento();
		public abstract void RestoreMemento(RequirementMemento memento);

		public abstract bool ContainsKey(Guid id);
		public abstract bool RemoveKey(Guid id);
		public abstract void ReplaceKey(Guid id, Guid otherId);

		public static Requirement CreateFromMemento(RequirementMemento memento)
		{
			Requirement newReq = null;
			if (memento is SimpleRequirementMemento)
			{
				newReq = new SimpleRequirement();
			}
			if (memento is ComplexRequirementMemento)
			{
				newReq = new ComplexRequirement();
			}

			if (newReq != null)
			{
				newReq.RestoreMemento(memento);
			}

			return newReq;
		}

		public RequirementObjectType myObjectType = RequirementObjectType.Undefined;
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
