
using System.Collections.Generic;
using Verifier.ItemRules;

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
