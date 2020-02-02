using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using Common.Utils;

namespace Common.Node
{
	public enum NodeType
	{
		Undefined = 0,
		Blank,
		Lock,
		RandomKey,
		EventKey,
	}

	[JsonConverter(typeof(NodeConverter))]
	public abstract class NodeBase
	{
		public NodeBase()
		{
			id = Guid.NewGuid();
		}

		public Guid id;

		public NodeType myNodeType;

		public Vector2 myPos;

		public List<Guid> myConnectionIds = new List<Guid>();
		[NonSerialized]
		public List<NodeBase> myConnections = new List<NodeBase>();

		public void FormConnections(List<NodeBase> nodes)
		{
			myConnections.Clear();
			foreach(var id in myConnectionIds)
			{
				var connection = nodes.FirstOrDefault(node => node.id == id);
				if(connection != null)
				{
					myConnections.Add(connection);
				}
			}
		}

		public void CreateConnection(NodeBase otherNode)
		{
			if (!myConnectionIds.Contains(otherNode.id))
			{
				myConnectionIds.Add(otherNode.id);
				myConnections.Add(otherNode);
			}
		}

		public void RemoveConnection(NodeBase otherNode)
		{
			if (myConnectionIds.Contains(otherNode.id))
			{
				myConnectionIds.Remove(otherNode.id);
				myConnections.Remove(otherNode);
			}
		}
	}

	//---------------------------------
	// JsonSerialization
	//---------------------------------
	public class BaseSpecifiedConcreteClassConverter : DefaultContractResolver
	{
		protected override JsonConverter ResolveContractConverter(Type objectType)
		{
			if (typeof(NodeBase).IsAssignableFrom(objectType) && !objectType.IsAbstract)
				return null; // pretend TableSortRuleConvert is not specified (thus avoiding a stack overflow)
			return base.ResolveContractConverter(objectType);
		}
	}

	public class NodeConverter : JsonConverter
	{
		static JsonSerializerSettings SpecifiedSubclassConversion = new JsonSerializerSettings() { ContractResolver = new BaseSpecifiedConcreteClassConverter() };

		public override bool CanConvert(Type objectType)
		{
			return (objectType == typeof(NodeBase));
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			JObject jo = JObject.Load(reader);
			switch (jo["myNodeType"].Value<int>())
			{
				case 1:
					return JsonConvert.DeserializeObject<BlankNode>(jo.ToString(), SpecifiedSubclassConversion);
				case 2:
					return JsonConvert.DeserializeObject<LockNode>(jo.ToString(), SpecifiedSubclassConversion);
				case 3:
					return JsonConvert.DeserializeObject<RandomKeyNode>(jo.ToString(), SpecifiedSubclassConversion);
				case 4:
					return JsonConvert.DeserializeObject<EventKeyNode>(jo.ToString(), SpecifiedSubclassConversion);
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
