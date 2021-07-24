
using System.Collections.Generic;
using Verifier.ItemRules;

namespace Randomizer
{
	public class FillOptions
	{
        public enum GameCompletion { NoLogic, Beatable, AllItems }
        public enum ItemSwap { Unchanged, LocalPool, GlobalPool }

        public GameCompletion gameCompletion;
        public ItemSwap majorSwap;
        public ItemSwap minorSwap;

        public bool noEarlyPbs = false;

        public int SprawlFactor = 0;

        public List<ItemRuleBase> itemRules = new List<ItemRuleBase>();
	}
}
