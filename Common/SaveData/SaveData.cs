using Common.Key;
using Common.Node;
using System;
using System.Collections.Generic;

namespace Common.SaveData
{
	[Serializable]
	public class SaveData
	{
		public Version version;
		public List<NodeBase> Nodes = new List<NodeBase>();
		public Dictionary<string, Dictionary<Guid, BaseKey>> BasicKeys = new Dictionary<string, Dictionary<Guid, BaseKey>>();
		public Dictionary<Guid, ComplexKey> CustomKeys = new Dictionary<Guid, ComplexKey>();
	}
}
