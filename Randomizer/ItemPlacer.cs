using Common.Key;
using Common.Node;
using Common.SaveData;
using Randomizer.ItemRules;
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

        private ILookup<Guid, string> itemBlockedLocationRules = null;
        private ILookup<Guid, string> itemRequiredLocationRules = null;

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

            itemRequiredLocationRules = options.itemRules.Where(rest => rest is ItemRuleInLocation).Select(rest => rest as ItemRuleInLocation).ToLookup(rest => rest.ItemId, rest => rest.LocationIdentifier);
            itemBlockedLocationRules = options.itemRules.Where(rest => rest is ItemRuleNotInLocation).Select(rest => rest as ItemRuleNotInLocation).ToLookup(rest => rest.ItemId, rest => rest.LocationIdentifier);
            
            searcher = new FillSearcher();

			var reachableKeys = new List<NodeBase>();
			var retracableKeys = new List<NodeBase>();
            var restrictedItems = new List<Guid>();

            var searchDepth = 0;

            while(true)
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

                var itemDepthRestrictions = options.itemRules.Where(rest => rest is ItemRuleRestrictedBeforeDepth depthRest && depthRest.SearchDepth > searchDepth);
                if (itemDepthRestrictions.Any())
                {
                    restrictedItems.AddRange(itemDepthRestrictions.Select(rest => rest.ItemId));
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

                var randomizedLocations = retracableKeys.Where(key => key is RandomKeyNode randomNode).Select(key => key as RandomKeyNode).OrderBy(x => x.id).ToList();
                var randomizedLocationNames = randomizedLocations.Select(loc => loc.myRandomKeyIdentifier);

                // Pick up any items already filled in on the map and update search before placing any items
                var preFilledLocations = randomizedLocations.Where(loc => loc.GetKey() != null);

                if (preFilledLocations.Any())
                {
                    inventory.myNodes.AddRange(preFilledLocations);
                    continue;
                }

                // Find which possible keys would expand number of retracable nodes
                var relevantKeys = FindRelevantKeys(inventory, restrictedItems, reachableKeys.Except(retracableKeys), pool);

                // Filter out any keys that by rules cannot be placed in any available location
                var filteredKeys = relevantKeys.Where(key => !itemBlockedLocationRules.Contains(key) || randomizedLocationNames.Any(loc => !itemBlockedLocationRules[key].Contains(loc)))
                    .Where(key => !itemRequiredLocationRules.Contains(key) || pool.CountKey(key) > 1 || randomizedLocationNames.Any(loc => itemRequiredLocationRules[key].Contains(loc)))
                    .ToList();

				var relevantNames = filteredKeys.Select(key => KeyManager.GetKey(key).Name).ToList();
				if (!filteredKeys.Any())
					break;

                // Get items that are prioritized according to item rules
                var prioritizedItems = options.itemRules.Where(rest => rest is ItemRulePrioritizedAfterDepth depthRest && depthRest.SearchDepth <= searchDepth)
                    .Select(rest => rest.ItemId)
                    .Where(item => !restrictedItems.Contains(item) && !inventory.ContainsKey(item))
                    .ToList();

                // Correlate prioritized and relevant keys to select a relevant key to place
                var prioritizedRelevantKeys = filteredKeys.Intersect(prioritizedItems);
                var selectedRelevantKey = prioritizedRelevantKeys.Any() ? pool.PullAmong(prioritizedRelevantKeys, random) : pool.PullAmong(filteredKeys, random);

                // Filter out available locations where selected key cannot be placed
                var filteredLocations = randomizedLocations;

                if(itemRequiredLocationRules.Contains(selectedRelevantKey) && pool.CountKey(selectedRelevantKey) == 1)
                {
                    filteredLocations = filteredLocations.Where(loc => itemRequiredLocationRules[selectedRelevantKey].Contains(loc.myRandomKeyIdentifier)).OrderBy(x => x.id).ToList();
                }

                if(itemBlockedLocationRules.Contains(selectedRelevantKey))
                {
                    filteredLocations = filteredLocations.Where(loc => !itemBlockedLocationRules[selectedRelevantKey].Contains(loc.myRandomKeyIdentifier)).OrderBy(x => x.id).ToList();
                }

                // Pick out one random accessible location, place the selected key there and add that item to inventory
                var relevantLocation = filteredLocations.ElementAt(random.Next(filteredLocations.Count));
				randomizedLocations.Remove(relevantLocation);
				itemMap.Add(relevantLocation.myRandomKeyIdentifier, selectedRelevantKey);
				inventory.myNodes.Add(relevantLocation);

                prioritizedItems.Remove(selectedRelevantKey);

                // Only increase searchDepth if an actual item is placed (debatable if this is the correct approach)
                searchDepth++;

                FulfillRequiredLocationRules(inventory, itemMap, pool, random);

                // Fill remaining accessible locations with random items
                foreach (var node in randomizedLocations)
				{
                    // This is possible through Required Location Rules
                    if (itemMap.ContainsKey(node.myRandomKeyIdentifier))
                    {
                        inventory.myNodes.Add(node);
                        continue;
                    }

                    var filteredPrioritizedItems = prioritizedItems.Where(key => !itemBlockedLocationRules.Contains(key) || !itemBlockedLocationRules[key].Contains(node.myRandomKeyIdentifier))
                        .Where(key => !itemRequiredLocationRules.Contains(key) || pool.CountKey(key) > 1 || itemRequiredLocationRules[key].Contains(node.myRandomKeyIdentifier));

                    if (filteredPrioritizedItems.Any())
                    {                        
                        var randomKey = pool.PullAmong(filteredPrioritizedItems, random);
                        prioritizedItems.Remove(randomKey);
                        itemMap.Add(node.myRandomKeyIdentifier, randomKey);
                        inventory.myNodes.Add(node);
                    }
                    else
                    {
                        // Add all items that cannot be in this location to restrictedItems
                        var locationRestrictedItems = restrictedItems.Union(itemBlockedLocationRules.Where(grouping => grouping.Contains(node.myRandomKeyIdentifier)).Select(grouping => grouping.Key))
                            .Union(itemRequiredLocationRules.Where(grouping => !grouping.Contains(node.myRandomKeyIdentifier)).Select(grouping => grouping.Key).Where(key => pool.CountKey(key) <= 1));

                        var selectedKey = pool.PullExcept(locationRestrictedItems, random);
                        if (selectedKey != Guid.Empty)
                        {
                            itemMap.Add(node.myRandomKeyIdentifier, selectedKey);
                            inventory.myNodes.Add(node);
                        }
                    }

                    FulfillRequiredLocationRules(inventory, itemMap, pool, random);
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
                restrictedAndEmpty.Add(StaticKeys.Nothing);

                FillRandomly(restrictedAndEmpty, inventory, itemMap, pool, random);
            }

            // Fill remaining reachable nodes randomly
            FillRandomly(restrictedItems, inventory, itemMap, pool, random);

            // Fill all remaining (unreachable) locations with any remaining items
            var remainingNodes = keyNodes.AsParallel().Where(node => node is RandomKeyNode randomNode && !itemMap.ContainsKey(randomNode.myRandomKeyIdentifier)).Select(key => key as RandomKeyNode).OrderBy(x => x.id).ToList();

			foreach (var node in remainingNodes)
			{
                if (itemMap.ContainsKey(node.myRandomKeyIdentifier))
                {
                    continue;
                }

                itemMap.Add(node.myRandomKeyIdentifier, pool.Pull(random));

                FulfillRequiredLocationRules(inventory, itemMap, pool, random);
            }

			return itemMap;
		}

        private void FulfillRequiredLocationRules(Inventory inventory, Dictionary<string, Guid> itemMap, ItemPool pool, Random random)
        {
            var itemsWithOpenLocationRule = itemRequiredLocationRules.Where(group => !group.Where(loc => itemMap.ContainsKey(loc) && itemMap[loc] == group.Key).Any());
            var itemsInPool = itemsWithOpenLocationRule.Where(item => pool.AvailableItems().Contains(item.Key)).ToList();

            while(itemsInPool.Any())
            {
                var curItem = itemsInPool.FirstOrDefault();
                var relatedItems = itemsInPool.Where(grouping => grouping.Intersect(curItem).Any()).OrderBy(x => x.Key).ToList();

                var availableLocations = curItem.Where(loc => !itemMap.ContainsKey(loc)).OrderBy(x => x);
                if(availableLocations.Count() == relatedItems.Count())
                {
                    foreach(var location in availableLocations)
                    {
                        var item = relatedItems.ElementAt(random.Next(relatedItems.Count()));
                        itemMap.Add(location, item.Key);
                        pool.Pull(item.Key);
                        relatedItems.Remove(item);
                    }
                }

                itemsInPool.RemoveAll(item => relatedItems.Contains(item));
            }
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

                var reachableRandomNodes = reachableNodes.Where(node => node is RandomKeyNode).Select(node => node as RandomKeyNode).OrderBy(x => x.id);

                var nodesToAdd = new List<NodeBase>();

                foreach (var node in reachableRandomNodes)
                {
                    if (itemMap.ContainsKey(node.myRandomKeyIdentifier))
                    {
                        nodesToAdd.Add(node);
                    }
                    else
                    { 
                        // Add all items that cannot be in this location to restrictedItems
                        var locationRestrictedItems = restrictedItems.Union(itemBlockedLocationRules.Where(grouping => grouping.Contains(node.myRandomKeyIdentifier)).Select(grouping => grouping.Key))
                            .Union(itemRequiredLocationRules.Where(grouping => !grouping.Contains(node.myRandomKeyIdentifier)).Select(grouping => grouping.Key).Where(key => pool.CountKey(key) <= 1));
                        
                        if (!pool.AvailableItems().Except(locationRestrictedItems).Any())
                            return;

                        var selectedKey = pool.PullExcept(locationRestrictedItems, random);
                        if (selectedKey != Guid.Empty)
                        {
                            itemMap.Add(node.myRandomKeyIdentifier, selectedKey);
                            nodesToAdd.Add(node);
                        }

                        FulfillRequiredLocationRules(inventory, itemMap, pool, random);
                    }
                }

                if (!nodesToAdd.Any())
                    break;

                inventory.myNodes.AddRange(nodesToAdd);
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
