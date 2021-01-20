using Randomizer.ItemRules;
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

        public List<ItemRuleBase> itemRules = new List<ItemRuleBase>();
	}
}
