using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Randomizer
{
	public class FillOptions
	{
        public enum GameCompletion { Unchanged, Beatable, AllItems }

        public GameCompletion gameCompletion;
		public bool noEarlyPbs = false;
		public Dictionary<Guid, int> maximumBatchRestrictions = new Dictionary<Guid, int>();
		public Dictionary<Guid, int> minimumBatchRestrictions = new Dictionary<Guid, int>();
	}
}
