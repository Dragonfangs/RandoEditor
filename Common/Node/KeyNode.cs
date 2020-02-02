using Common.Key;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Node
{
	public abstract class KeyNode : NodeBase
	{
		public abstract BaseKey GetKey();
	}
}
