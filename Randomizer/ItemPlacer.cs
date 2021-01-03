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

        public Dictionary<string, Guid> FillLocations(SaveData someData, FillOptions options, Dictionary<string, Guid> itemMap)
        {
            var pool = new ItemPool();
            pool.CreatePool(someData);

            return FillLocations(someData, options, pool, itemMap);
        }

        public Dictionary<string, Guid> FillLocations(SaveData someData, FillOptions options, ItemPool pool, Random random)
        {
            var startingInventory = new Inventory();

            return FillLocations(someData, options, pool, startingInventory, random);
        }

        public Dictionary<string, Guid> FillLocations(SaveData someData, FillOptions options, ItemPool pool, Dictionary<string, Guid> itemMap)
        {
            var startingInventory = new Inventory();

            return FillLocations(someData, options, pool, startingInventory, itemMap);
        }

        public Dictionary<string, Guid> FillLocations(SaveData someData, FillOptions options, ItemPool pool, Inventory startingInventory)
		{
			var random = new Random();

			return FillLocations(someData, options, pool, startingInventory, random);
		}
        
        public Dictionary<string, Guid> FillLocations(SaveData someData, FillOptions options, Inventory startingInventory, Random random)
		{
			var pool = new ItemPool();
			pool.CreatePool(someData);

			return FillLocations(someData, options, pool, startingInventory, random);
		}

        public Dictionary<string, Guid> FillLocations(SaveData someData, FillOptions options, Inventory startingInventory, Dictionary<string, Guid> itemMap)
        {
            var pool = new ItemPool();
            pool.CreatePool(someData);

            return FillLocations(someData, options, pool, startingInventory, itemMap);
        }

        public Dictionary<string, Guid> FillLocations(SaveData someData, FillOptions options, Random random, Dictionary<string, Guid> itemMap)
        {
            var pool = new ItemPool();
            pool.CreatePool(someData);

            return FillLocations(someData, options, pool, random, itemMap);
        }

        public Dictionary<string, Guid> FillLocations(SaveData someData, FillOptions options, ItemPool pool, Inventory startingInventory, Random random)
        {
            var itemMap = new Dictionary<string, Guid>();
            return FillLocations(someData, options, pool, startingInventory, random, itemMap);
        }

        public Dictionary<string, Guid> FillLocations(SaveData someData, FillOptions options, ItemPool pool, Inventory startingInventory, Dictionary<string, Guid> itemMap)
        {
            var random = new Random();
            return FillLocations(someData, options, pool, startingInventory, random, itemMap);
        }

        public Dictionary<string, Guid> FillLocations(SaveData someData, FillOptions options, ItemPool pool, Random random, Dictionary<string, Guid> itemMap)
        {
            var startingInventory = new Inventory();
            return FillLocations(someData, options, pool, startingInventory, random, itemMap);
        }

        public Dictionary<string, Guid> FillLocations(SaveData someData, FillOptions options, Inventory startingInventory, Random random, Dictionary<string, Guid> itemMap)
        {
            var pool = new ItemPool();
            return FillLocations(someData, options, pool, startingInventory, random, itemMap);
        }

        public Dictionary<string, Guid> FillLocations(SaveData someData, FillOptions options, ItemPool pool, Inventory startingInventory, Random random, Dictionary<string, Guid> itemMap)
		{
			var inventory = new Inventory(startingInventory);

			KeyManager.SetRandomKeyMap(itemMap);
			
			var nodeCollection = new NodeCollection();
			nodeCollection.InitializeNodes(someData);

			keyNodes = nodeCollection.myNodes.Where(node => node is KeyNode).ToList();
			var eventNodes = keyNodes.Where(node => node is EventKeyNode).Select(node => node as EventKeyNode);
			startNode = eventNodes.FirstOrDefault(node => node.myKeyId == StaticKeys.GameStart);
			endNode = eventNodes.FirstOrDefault(node => node.myKeyId == StaticKeys.GameFinish);

			searcher = new FillSearcher();

			var reachableKeys = new List<NodeBase>();
			var retracableKeys = new List<NodeBase>();
            var restrictedItems = new List<Guid>();
            while (true)
			{
                // Beatable conditional
                if (options.gameCompletion == FillOptions.GameCompletion.Beatable && inventory.myNodes.Contains(endNode))
                    break;

                // Do not place any power bombs until obtaining powered suit
                restrictedItems.Clear();
				if (options.noEarlyPbs && !inventory.ContainsKey(StaticKeys.CharlieDefeated))
				{
					restrictedItems.Add(StaticKeys.PowerBombs);
				}

                // Find all nodes that can be reached with current inventory
				reachableKeys.RemoveAll(node => inventory.myNodes.Contains(node));
				reachableKeys.AddRange(searcher.ContinueSearch(startNode, inventory, node => (node is KeyNode) && !inventory.myNodes.Contains(node)));

				// Special case to handle how chozodia area is built 
				// Specifically can't get out of it without power bombs, which creates awkward dynamics regarding the placements of said power bombs)
				// Special case triggers on reaching charlie without picking up any power bombs
				if (reachableKeys.Any(key => key is EventKeyNode eventKey && eventKey.myKeyId == StaticKeys.CharlieDefeated) &&
					!inventory.ContainsKey(StaticKeys.PowerBombs))
				{
                    FillRandomly(restrictedItems, inventory, itemMap, pool, random);

                    if (!inventory.ContainsKey(StaticKeys.CharlieDefeated))
                    {
                        // Unless Charlie was for some reason reached during fill, start new search with Charlie as new start node
                        startNode = eventNodes.FirstOrDefault(x => x.myKeyId == StaticKeys.CharlieDefeated);
                        inventory.myNodes.Add(startNode);
                        searcher = new FillSearcher();
                    }

					continue;
				}

				// Find all reachable keys that are also possible to get back from
                // (End node is always considered retracable, since being able to reach it at all is just akin to "being in go mode")
				retracableKeys.RemoveAll(node => inventory.myNodes.Contains(node));
				var notRetracable = reachableKeys.Except(retracableKeys);
				retracableKeys.AddRange(notRetracable.AsParallel().Where(node => node == endNode || NodeTraverser.PathExists(node, startNode, node is EventKeyNode ? inventory.Expand(node) : inventory)).ToList());

				if (!retracableKeys.Any())
					break;

				// If any events can be reached, add to inventory and update search before continuing
				var retracableEvents = retracableKeys.Where(node => node is EventKeyNode).ToList();
				if (retracableEvents.Any())
				{
					inventory.myNodes.AddRange(retracableEvents);
					continue;
				}

                var randomizedLocations = retracableKeys.Where(key => key is RandomKeyNode randomNode).Select(key => key as RandomKeyNode).ToList();

                // Pick up any items already filled in on the map and update search before placing any items
                var preFilledLocations = randomizedLocations.Where(loc => loc.GetKey() != null);

                if (preFilledLocations.Any())
                {
                    inventory.myNodes.AddRange(preFilledLocations);
                    continue;
                }

                // Find which possible keys would expand number of retracable nodes
                var relevantKeys = FindRelevantKeys(inventory, restrictedItems, reachableKeys.Except(retracableKeys), pool);
				// var relevantNames = relevantKeys.Select(key => KeyManager.GetKey(key).Name).ToList();
				if (!relevantKeys.Any())
					break;

				// Pick out one random accessible location, place one random of the relevant keys there and add that item to inventory
				var relevantLocation = randomizedLocations.ElementAt(random.Next(randomizedLocations.Count));
				randomizedLocations.Remove(relevantLocation);
				itemMap.Add(relevantLocation.myRandomKeyIdentifier, pool.PullAmong(relevantKeys, random));
				inventory.myNodes.Add(relevantLocation);

				// Fill remaining accessible locations with random items
				foreach (var node in randomizedLocations)
				{
					itemMap.Add(node.myRandomKeyIdentifier, pool.PullExcept(restrictedItems, random));
					inventory.myNodes.Add(node);
				}

				// Go back to start of loop to continue search with updated inventory
			}

            // POST-FILL:
            // Reachable if seed is beatable or if it ran out of possible relevant keys
            // Fill in remaining locations as well as possible without breaking any restrictions
            
            // Do not place any power bombs until obtaining powered suit
            restrictedItems.Clear();
            if (options.noEarlyPbs && !inventory.ContainsKey(StaticKeys.CharlieDefeated))
            {
                restrictedItems.Add(StaticKeys.PowerBombs);
            }

            if (options.gameCompletion == FillOptions.GameCompletion.AllItems)
			{
                // Prioritize filling in non-empty items
                var restrictedAndEmpty = new List<Guid>(restrictedItems);
                restrictedAndEmpty.Add(Guid.Empty);

                FillRandomly(restrictedAndEmpty, inventory, itemMap, pool, random);
            }

            // Fill remaining reachable nodes randomly
            FillRandomly(restrictedItems, inventory, itemMap, pool, random);

            // Fill all remaining (unreachable) locations with any remaining items
            var remainingNodes = keyNodes.AsParallel().Where(node => node is RandomKeyNode randomNode && !itemMap.ContainsKey(randomNode.myRandomKeyIdentifier)).Select(key => key as RandomKeyNode).ToList();

			foreach (var node in remainingNodes)
			{
                itemMap.Add(node.myRandomKeyIdentifier, pool.Pull(random));
			}

			return itemMap;
		}

        private void FillRandomly(List<Guid> restrictedItems, Inventory inventory, Dictionary<string, Guid> itemMap, ItemPool pool, Random random)
        {
            var reachableNodes = new List<NodeBase>();
            var localSearcher = new FillSearcher();

            // Fill all still reachable random nodes
            while (true)
            {
                reachableNodes.RemoveAll(node => inventory.myNodes.Contains(node));
                reachableNodes.AddRange(localSearcher.ContinueSearch(startNode, inventory, node => (node is KeyNode) && !inventory.myNodes.Contains(node)));

                // Find all reachable events that are also possible to get back from
                var retracableEvents = reachableNodes.AsParallel().Where(node => node is EventKeyNode && NodeTraverser.PathExists(node, startNode, inventory.Expand(node))).ToList();

                // If any events can be reached, add to inventory and update search before continuing
                if (retracableEvents.Any())
                {
                    inventory.myNodes.AddRange(retracableEvents);
                    continue;
                }

                var reachableRandomNodes = reachableNodes.Where(node => node is RandomKeyNode).Select(node => node as RandomKeyNode);

                if (!reachableRandomNodes.Any())
                    return;

                foreach (var node in reachableRandomNodes)
                {
                    if (!itemMap.ContainsKey(node.myRandomKeyIdentifier))
                    {
                        if (!pool.AvailableItems().Except(restrictedItems).Any())
                            return;

                        itemMap.Add(node.myRandomKeyIdentifier, pool.PullExcept(restrictedItems, random));
                    }

                    inventory.myNodes.Add(node);
                }
            }
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
