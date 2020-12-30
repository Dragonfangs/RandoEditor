using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Randomizer
{
	public class FillOptions
	{
		public int GameCompletion;
		public bool noEarlyPbs = false;
		public Dictionary<Guid, int> maximumBatchRestrictions = new Dictionary<Guid, int>();
		public Dictionary<Guid, int> minimumBatchRestrictions = new Dictionary<Guid, int>();
	}
}
