using Common.Key;
using Common.SaveData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Randomizer
{
	public class ItemPool
	{
		List<Guid> myAvailableItems = new List<Guid>();
		
		public void CreatePool(SaveData someData)
		{
			myAvailableItems.Clear();

			myAvailableItems.AddRange(KeyManager.GetRandomKeys().Select(key => key.Id));

			// Missiles
			for (int i = 0; i < 49; i++)
			{
				myAvailableItems.Add(StaticKeys.Missile);
			}

			// Supers
			for (int i = 0; i < 14; i++)
			{
				myAvailableItems.Add(StaticKeys.SuperMissile);
			}

			// Power Bombs
			for (int i = 0; i < 8; i++)
			{
				myAvailableItems.Add(StaticKeys.PowerBombs);
			}

			// E-Tank
			for (int i = 0; i < 11; i++)
			{
				myAvailableItems.Add(StaticKeys.ETank);
			}
		}

		public void SetItems(List<Guid> someItems)
		{
			myAvailableItems = new List<Guid>(someItems);
		}

		public List<Guid> AvailableItems()
		{
			return new List<Guid>(myAvailableItems);
		}

        public int CountKey(Guid key)
        {
            return myAvailableItems.Count(x => x == key);
        }

		// Get a random item from the pool without removing it
		public Guid PeekExcept(IEnumerable<Guid> itemList, Random random)
		{
			var filteredItems = myAvailableItems.Where(item => !itemList.Contains(item));

			if (!filteredItems.Any())
				return Guid.Empty;

			var index = random.Next(filteredItems.Count());
			var returnItem = filteredItems.ElementAt(index);

			return returnItem;
		}

		// Get a random item from the pool without removing it
		public Guid PeekAmong(IEnumerable<Guid> itemList, Random random)
		{
			var filteredItems = myAvailableItems.Where(item => itemList.Contains(item));

			if (!filteredItems.Any())
				return Peek(random);

			var index = random.Next(filteredItems.Count());
			var returnItem = filteredItems.ElementAt(index);

			return returnItem;
		}

		// Get a random item from the pool without removing it
		public Guid Peek(Random random)
		{
			if (myAvailableItems.Count < 1)
				return Guid.Empty;

			var index = random.Next(myAvailableItems.Count);
			var item = myAvailableItems.ElementAt(index);

			return item;
		}

		// Remove specific item out of pool
		public bool Pull(Guid id)
		{
			var item = myAvailableItems.FirstOrDefault(key => key == id);
			return myAvailableItems.Remove(item);
		}

		// Get random item from the pool and remove it
		public Guid PullExcept(IEnumerable<Guid> itemList, Random random)
		{
			var item = PeekExcept(itemList, random);
			Pull(item);

			return item;
		}

		// Get random item from the pool and remove it
		public Guid PullAmong(IEnumerable<Guid> itemList, Random random)
		{
			var item = PeekAmong(itemList,random);
			Pull(item);

			return item;
		}

		// Get random item from the pool and remove it
		public Guid Pull(Random random)
		{
			var item = Peek(random);
			Pull(item);

			return item;
		}

		public void RemoveRandomItems(int count, Random random)
		{
            RemoveRandomItemsExcept(count, random, new List<Guid>());
		}

        public void RemoveRandomItemsExcept(int count, Random random, List<Guid> exceptItems)
        {
            foreach (var item in exceptItems)
            {
                Pull(item);
            }

            for (int i = 0; i < count && myAvailableItems.Count > 0; i++)
            {
                Pull(random);
            }

            foreach(var item in exceptItems)
            {
                myAvailableItems.Add(item);
            }

            while(myAvailableItems.Count < 100)
            {
                myAvailableItems.Add(StaticKeys.Nothing);
            }
        }
    }
}
