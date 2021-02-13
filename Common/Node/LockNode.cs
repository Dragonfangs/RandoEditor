using System;
using Common.Key.Requirement;

namespace Common.Node
{
	[Serializable]
	public class LockNode : NodeBase
	{
		public LockNode()
		{
			id = Guid.NewGuid();

			myNodeType = NodeType.Lock;
		}

        public override string Name()
        {
            return "Lock";
        }

        public ComplexRequirement myRequirement = new ComplexRequirement();
	}
}
