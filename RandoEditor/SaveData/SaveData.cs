using RandoEditor.Key;
using RandoEditor.Node;
using System;
using System.Collections.Generic;

namespace RandoEditor.SaveData
{
	[Serializable]
	public class SaveData
	{
		public List<PathNode> Nodes = new List<PathNode>();
		public Dictionary<string, Dictionary<Guid, BaseKey>> BasicKeys = new Dictionary<string, Dictionary<Guid, BaseKey>>();
		public Dictionary<Guid, ComplexKey> CustomKeys = new Dictionary<Guid, ComplexKey>();
	}
}
