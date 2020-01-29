﻿using Verifier.Key;
using Verifier.Node;
using System;
using System.Collections.Generic;

namespace Verifier.SaveData
{
	[Serializable]
	public class SaveData
	{
		public Version version;
		public List<PathNode> Nodes = new List<PathNode>();
		public Dictionary<string, Dictionary<Guid, BaseKey>> BasicKeys = new Dictionary<string, Dictionary<Guid, BaseKey>>();
		public Dictionary<Guid, ComplexKey> CustomKeys = new Dictionary<Guid, ComplexKey>();
	}
}