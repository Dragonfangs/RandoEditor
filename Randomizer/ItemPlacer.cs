using Common.Key;
using Common.Node;
using Common.SaveData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verifier;
using Verifier.Key;

namespace Randomizer
{
	public class ItemPlacer
	{
		private List<NodeBase> keyNodes = null;

		private EventKeyNode startNode = null;
		private EventKeyNode endNode = null;

		private FillSearcher searcher = null;

		public Dictionary<string, Guid> FillLocations(SaveData someData, FillOptions options)
		{
			var pool = new ItemPool();
			pool.CreatePool(someData);

			return FillLocations(someData, options, pool);
		}

		public Dictionary<string, Guid> FillLocations(SaveData someData, FillOptions options, ItemPool pool)
		{
			var startingInventory = new Inventory();

			return FillLocations(someData, options, pool, startingInventory);
		}

		public Dictionary<string, Guid> FillLocations(SaveData someData, FillOptions options, Inventory startingInventory)
		{
			var pool = new ItemPool();
			pool.CreatePool(someData);

			return FillLocations(someData, options, pool, startingInventory);
		}

		public Dictionary<string, Guid> FillLocations(SaveData someData, FillOptions options, Random random)
		{
			var pool = new ItemPool();
			pool.CreatePool(someData);

			return FillLocations(someData, options, pool, random);
		}

		public Dictionary<string, Guid> FillLocations(SaveData someData, FillOptions options, ItemPool pool, Inventory startingInventory)
		{
			var random = new Random();

			return FillLocations(someData, options, pool, startingInventory, random);
		}

		public Dictionary<string, Guid> FillLocations(SaveData someData, FillOptions options, ItemPool pool, Random random)
		{
			var startingInventory = new Inventory();

			return FillLocations(someData, options, pool, startingInventory, random);
		}

		public Dictionary<string, Guid> FillLocations(SaveData someData, FillOptions options, Inventory startingInventory, Random random)
		{
			var pool = new ItemPool();
			pool.CreatePool(someData);

			return FillLocations(someData, options, pool, startingInventory, random);
		}

		public Dictionary<string, Guid> FillLocations(SaveData someData, FillOptions options, ItemPool pool, Inventory startingInventory, Random random)
		{
			var itemMap = new Dictionary<string, Guid>();
			var inventory = new Inventory(startingInventory);

			KeyManager.SetRandomKeyMap(itemMap);
			
			var nodeCollection = new NodeCollection();
			nodeCollection.InitializeNodes(someData);

			keyNodes = nodeCollection.myNodes.Where(node => node is KeyNode).ToList();
			var eventNodes = keyNodes.Where(node => node is EventKeyNode).Select(node => node as EventKeyNode);
			startNode = eventNodes.FirstOrDefault(x => x.myKeyId == StaticKeys.GameStart);
			endNode = eventNodes.FirstOrDefault(x => x.myKeyId == StaticKeys.GameFinish);

			searcher = new FillSearcher();

			var reachableKeys = new List<NodeBase>();
			var retracableKeys = new List<NodeBase>();
			while (true)
			{
				// Do not place any power bombs until obtaining powered suit
				var restrictedItems = new List<Guid>();
				if (options.noEarlyPbs && !inventory.ContainsKey(StaticKeys.CharlieDefeated))
				{
					restrictedItems.Add(StaticKeys.PowerBombs);
				}

				reachableKeys.RemoveAll(node => inventory.myNodes.Contains(node));
				reachableKeys.AddRange(searcher.ContinueSearch(startNode, inventory, node => (node is KeyNode) && !inventory.myNodes.Contains(node)));

				// Special case to handle how chozodia area is built 
				// Specifically can't get out of it without power bombs, which creates awkward dynamics regarding the placements of said power bombs)
				// Special case triggers on reaching charlie without picking up any power bombs
				if (reachableKeys.Any(key => key is EventKeyNode eventKey && eventKey.myKeyId == StaticKeys.CharlieDefeated) &&
					!inventory.ContainsKey(StaticKeys.PowerBombs))
				{
					// Fill all still reachable random nodes
					while (reachableKeys.Any(key => key is RandomKeyNode))
					{
						var locations = reachableKeys.Where(key => key is RandomKeyNode).Select(key => key as RandomKeyNode).ToList();

						foreach (var key in locations)
						{
							itemMap.Add(key.myRandomKeyIdentifier, pool.PullExcept(restrictedItems, random));
							inventory.myNodes.Add(key);
							reachableKeys.Remove(key);
						}

						reachableKeys.AddRange(searcher.ContinueSearch(startNode, inventory, node => (node is KeyNode) && !inventory.myNodes.Contains(node)));
					}

					// Add all reachable nodes to inventory (should only be charlie at this point... probably)
					inventory.myNodes.AddRange(reachableKeys);
					reachableKeys.Clear();
					retracableKeys.Clear();

					// Start new search with Charlie as new start node
					startNode = eventNodes.FirstOrDefault(x => x.myKeyId == StaticKeys.CharlieDefeated);
					searcher = new FillSearcher();

					continue;
				}

				// Find all reachable keys that are also possible to get back from
				retracableKeys.RemoveAll(node => inventory.myNodes.Contains(node));
				var notRetracable = reachableKeys.Except(retracableKeys);
				retracableKeys.AddRange(notRetracable.AsParallel().Where(node => node == endNode || NodeTraverser.PathExists(node, startNode, node is EventKeyNode ? inventory.Expand(node) : inventory)).ToList());

				if (!retracableKeys.Any())
					break;

				// If any events can be reached, add to inventory and update search before continuing
				var retracableEvents = retracableKeys.Where(node => node is EventKeyNode).ToList();
				if (retracableEvents.Any())
				{
					foreach(var node in retracableEvents)
					{
						inventory.myNodes.Add(node);
					}

					// Beatable conditional
					if (options.GameCompletion == 1 && inventory.myNodes.Contains(endNode))
						break;
					else
						continue;
				}

				// Find which possible keys would expand number of retracable nodes
				var relevantKeys = FindRelevantKeys(inventory, restrictedItems, reachableKeys.Except(retracableKeys), pool);
				var relevantNAmes = relevantKeys.Select(key => KeyManager.GetKey(key).Name).ToList();
				if (!relevantKeys.Any())
					break;
								
				var randomizedLocations = retracableKeys.Where(key => key is RandomKeyNode).Select(key => key as RandomKeyNode).ToList();

				// Pick out one random accessible location, place one random of the relevant keys there and add that item to inventory
				var relevantLocation = randomizedLocations.ElementAt(random.Next(randomizedLocations.Count - 1));
				randomizedLocations.Remove(relevantLocation);
				itemMap.Add(relevantLocation.myRandomKeyIdentifier, pool.PullAmong(relevantKeys, random));
				inventory.myNodes.Add(relevantLocation);

				// Fill remaining accessible locations with random items
				foreach (var key in randomizedLocations)
				{
					itemMap.Add((key as RandomKeyNode).myRandomKeyIdentifier, pool.PullExcept(restrictedItems, random));
					inventory.myNodes.Add(key);
				}

				// Go back to start of loop to continue search with updated inventory
			}

			if (options.GameCompletion == 2)
			{
				// Try to fill all remaining real items in reachable locations
				var retracableLocations = retracableKeys.Where(key => key is RandomKeyNode).Select(key => key as RandomKeyNode).ToList();

				while (retracableLocations.Any(loc => !inventory.myNodes.Contains(loc)) && pool.AvailableItems().Any(key => key != Guid.Empty))
				{
					var emptyLocations = retracableLocations.Where(loc => !inventory.myNodes.Contains(loc));
					var key = emptyLocations.ElementAt(random.Next(emptyLocations.Count() - 1)) as RandomKeyNode;
					itemMap.Add(key.myRandomKeyIdentifier, pool.PullExcept(new List<Guid> { Guid.Empty }, random));
					inventory.myNodes.Add(key);
				}
			}

			// Fill all remaining items in remaining locations
			var remainingKeys = keyNodes.AsParallel().Where(node => node is RandomKeyNode && !inventory.myNodes.Contains(node)).Select(key => key as RandomKeyNode).ToList();

			foreach (var key in remainingKeys)
			{
				itemMap.Add(key.myRandomKeyIdentifier, pool.Pull(random));
			}

			return itemMap;
		}

		public Dictionary<string, Guid> SearchPhase2(SaveData someData, FillOptions options, ItemPool pool, Inventory startingInventory, Random random)
		{
			return new Dictionary<string, Guid>();
		}

		private List<Guid> FindRelevantKeys(Inventory inventory, List<Guid> restrictedItems, IEnumerable<NodeBase> untracableKeys, ItemPool pool)
		{
			var returnList = new List<Guid>();
			foreach (var item in pool.AvailableItems().Distinct().Where(key => !restrictedItems.Contains(key)))
			{
				var testInventory = inventory.Expand(KeyManager.GetKey(item));

				var tempSearcher = new FillSearcher(searcher);

				var combinedReachableKeys = untracableKeys.Union(tempSearcher.ContinueSearch(startNode, testInventory, node => (node is KeyNode) && !testInventory.myNodes.Contains(node)));

				var retracableKeys = combinedReachableKeys.AsParallel().Where(node => node == endNode || NodeTraverser.PathExists(node, startNode, node is EventKeyNode ? testInventory.Expand(node) : testInventory)).ToList();

				if (retracableKeys.Any())
				{
					returnList.Add(item);
				}
			}

			return returnList;
		}
	}
}
