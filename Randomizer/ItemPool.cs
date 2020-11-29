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
		List<BaseKey> myAvailableItems = new List<BaseKey>();

		public void CreatePool(SaveData someData)
		{
			myAvailableItems.Clear();

			myAvailableItems.AddRange(KeyManager.GetRandomKeys());

			// Missiles
			for (int i = 0; i < 49; i++)
			{
				myAvailableItems.Add(KeyManager.GetKey(Guid.Parse("71c295d4-ba02-410b-8e4d-a53f8cec36fa")));
			}

			// Supers
			for (int i = 0; i < 14; i++)
			{
				myAvailableItems.Add(KeyManager.GetKey(Guid.Parse("ca562190-3a28-4a6a-b65a-9b3df94b6501")));
			}

			// Power Bombs
			for (int i = 0; i < 8; i++)
			{
				myAvailableItems.Add(KeyManager.GetKey(Guid.Parse("fde4a9a5-1b38-4c4a-a918-8556a55e8e98")));
			}

			// E-Tank
			for (int i = 0; i < 11; i++)
			{
				myAvailableItems.Add(KeyManager.GetKey(Guid.Parse("54d8e47c-b29b-440d-9a8b-bab7bc995f1d")));
			}
		}

		public Guid Pull(Random random)
		{
			var index = random.Next(myAvailableItems.Count-1);
			var item = myAvailableItems.ElementAt(index);
			myAvailableItems.RemoveAt(index);

			if (item == null)
				return Guid.Empty;

			return item.Id;
		}

		public void RemoveRandomItems(int count, Random random)
		{
			for(int i=0; i < count; i++)
			{
				Pull(random);
			}

			for (int i = 0; i < count; i++)
			{
				myAvailableItems.Add(null);
			}
		}
	}
}
