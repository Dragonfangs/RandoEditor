using System;

namespace Common.Node
{
	[Serializable]
	public class BlankNode : NodeBase
	{
		public BlankNode()
		{
			id = Guid.NewGuid();

			myNodeType = NodeType.Blank;
		}
	}
}
