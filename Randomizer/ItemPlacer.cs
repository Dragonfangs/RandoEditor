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

        private Dictionary<Guid, List<string>> itemBlockedLocationRules = null;
        private Dictionary<Guid, List<string>> itemRequiredLocationRules = null;
        private HashSet<string> restrictedLocations = null;

        private EventKeyNode startNode = null;
		private EventKeyNode endNode = null;

		private FillSearcher searcher = null;

        public string placeLog;

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
            placeLog = "";

			var inventory = new Inventory(startingInventory);

			KeyManager.SetRandomKeyMap(itemMap);
			
			var nodeCollection = new NodeCollection();
			nodeCollection.InitializeNodes(someData);

			keyNodes = nodeCollection.myNodes.Where(node => node is KeyNode).ToList();
			var eventNodes = keyNodes.Where(node => node is EventKeyNode).Select(node => node as EventKeyNode);
			startNode = eventNodes.FirstOrDefault(node => node.myKeyId == StaticKeys.GameStart);
			endNode = eventNodes.FirstOrDefault(node => node.myKeyId == StaticKeys.GameFinish);

            itemRequiredLocationRules = options.itemRules.Where(rest => rest is ItemRuleInLocation).Select(rest => rest as ItemRuleInLocation).ToLookup(rest => rest.ItemId, rest => rest.LocationIdentifier).ToDictionary(group => group.Key, group => group.ToList());
            itemBlockedLocationRules = options.itemRules.Where(rest => rest is ItemRuleNotInLocation).Select(rest => rest as ItemRuleNotInLocation).ToLookup(rest => rest.ItemId, rest => rest.LocationIdentifier).ToDictionary(group => group.Key, group => group.ToList()); ;
            restrictedLocations = new HashSet<string>();

            // Remove redundant rules
            foreach(var rule in itemBlockedLocationRules)
            {
                if(itemRequiredLocationRules.ContainsKey(rule.Key))
                {
                    var overlappingLocations = rule.Value.Where(loc => itemRequiredLocationRules[rule.Key].Contains(loc)).ToList();

                    if (overlappingLocations.Any())
                    {
                        placeLog += $"OverlappingBlockedRequiredRules for item {KeyManager.GetKey(rule.Key)?.Name ?? "Nothing"}: {overlappingLocations.Aggregate((i, j) => i + ", " + j)}{Environment.NewLine}";
                        itemRequiredLocationRules[rule.Key].RemoveAll(loc => overlappingLocations.Contains(loc));
                    }

                    if(!itemRequiredLocationRules[rule.Key].Any())
                    {
                        placeLog += $"Item {KeyManager.GetKey(rule.Key)?.Name ?? "Nothing"} left with no requirement rules, removed{Environment.NewLine}";
                        itemRequiredLocationRules.Remove(rule.Key);
                    }
                }
            }

            if (itemRequiredLocationRules.Any())
            {
                placeLog += $"RequiredLocationRules:{Environment.NewLine}{itemRequiredLocationRules.Select(group => $"{KeyManager.GetKey(group.Key)?.Name ?? "Nothing"} : {group.Value.Aggregate((i, j) => i + ", " + j)}").Aggregate((i, j) => i + Environment.NewLine + j)}";
                placeLog += Environment.NewLine;
            }

            if (itemBlockedLocationRules.Any())
            {
                placeLog += $"BlockedLocationRules:{Environment.NewLine}{itemBlockedLocationRules.Select(group => $"{KeyManager.GetKey(group.Key)?.Name ?? "Nothing"} : {group.Value.Aggregate((i, j) => i + ", " + j)}").Aggregate((i, j) => i + Environment.NewLine + j)}";
                placeLog += Environment.NewLine;
            }

            UpdateLocationRestrictions(inventory, itemMap, pool, random);

            searcher = new FillSearcher();

			var reachableKeys = new List<NodeBase>();
			var retracableKeys = new List<NodeBase>();
            var restrictedItems = new List<Guid>();

            var searchDepth = 0;

            while(true)
			{
                placeLog += $"Step start, depth: {searchDepth}{Environment.NewLine}";

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

                if (restrictedItems.Any())
                {
                    placeLog += $"Restricted Items: {restrictedItems.Select(key => KeyManager.GetKey(key)?.Name ?? "Nothing").Aggregate((i, j) => i + ", " + j)}{Environment.NewLine}";
                }

                // Find all nodes that can be reached with current inventory
				reachableKeys.RemoveAll(node => inventory.myNodes.Contains(node));
				reachableKeys.AddRange(searcher.ContinueSearch(startNode, inventory, node => (node is KeyNode) && !inventory.myNodes.Contains(node)));

                if (reachableKeys.Any())
                {
                    placeLog += $"Reachable keys: {reachableKeys.Select(node => node.Name()).Aggregate((i, j) => i + ", " + j)}{Environment.NewLine}";
                }

                // Special case to handle how chozodia area is built 
                // Specifically can't get out of it without power bombs, which creates awkward dynamics regarding the placements of said power bombs)
                // Special case triggers on reaching charlie without picking up any power bombs
                if (reachableKeys.Any(key => key is EventKeyNode eventKey && eventKey.myKeyId == StaticKeys.CharlieDefeated) &&
					!inventory.ContainsKey(StaticKeys.PowerBombs))
				{
                    placeLog += $"Charlie special condition{Environment.NewLine}";

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
                {
                    placeLog += $"Retracable keys empty{Environment.NewLine}";
                    break;
                }

                placeLog += $"Retracable keys: {retracableKeys.Select(node => node.Name()).Aggregate((i, j) => i + ", " + j)}{Environment.NewLine}";

                // If any events can be reached, add to inventory and update search before continuing
                var retracableEvents = retracableKeys.Where(node => node is EventKeyNode).ToList();
                if (retracableEvents.Any())
				{
                    placeLog += $"Retracable events: {retracableEvents.Select(node => node.Name()).Aggregate((i, j) => i + ", " + j)}{Environment.NewLine}";
                    inventory.myNodes.AddRange(retracableEvents);
					continue;
				}

                var randomizedLocations = retracableKeys.Where(key => key is RandomKeyNode randomNode).Select(key => key as RandomKeyNode).OrderBy(x => x.id).ToList();
                var randomizedLocationNames = randomizedLocations.Select(loc => loc.myRandomKeyIdentifier);

                // Pick up any items already filled in on the map and update search before placing any items
                var preFilledLocations = randomizedLocations.Where(loc => loc.GetKey() != null);

                if (preFilledLocations.Any())
                {
                    placeLog += $"Prefilled locations: {preFilledLocations.Select(node => node.Name()).Aggregate((i, j) => i + ", " + j)}{Environment.NewLine}";
                    inventory.myNodes.AddRange(preFilledLocations);
                    continue;
                }

                // Find which possible keys would expand number of retracable nodes
                var relevantKeys = FindRelevantKeys(inventory, restrictedItems, reachableKeys.Except(retracableKeys), pool);

                if (relevantKeys.Any())
                {
                    placeLog += $"Relevant Keys: {relevantKeys.Select(key => KeyManager.GetKey(key)?.Name ?? "Nothing").Aggregate((i, j) => i + ", " + j)}{Environment.NewLine}";
                }

                // Filter out any keys that by rules cannot be placed in any available location
                var filteredKeys = relevantKeys.Where(key => randomizedLocationNames.Any(location => KeyAllowedInLocation(key, itemMap, pool, location)));

                if (!filteredKeys.Any())
                {
                    placeLog += $"FilteredKeys empty{Environment.NewLine}";
                    break;
                }

                placeLog += $"Filtered Keys: {filteredKeys.Select(key => KeyManager.GetKey(key)?.Name ?? "Nothing").Aggregate((i, j) => i + ", " + j)}{Environment.NewLine}";

                // Get items that are prioritized according to item rules
                var prioritizedItems = options.itemRules.Where(rest => rest is ItemRulePrioritizedAfterDepth depthRest && depthRest.SearchDepth <= searchDepth)
                    .Select(rest => rest.ItemId)
                    .Where(item => !restrictedItems.Contains(item) && !inventory.ContainsKey(item))
                    .ToList();

                if (prioritizedItems.Any())
                {
                    placeLog += $"Prioritized Items: {prioritizedItems.Select(key => KeyManager.GetKey(key)?.Name ?? "Nothing").Aggregate((i, j) => i + ", " + j)}{Environment.NewLine}";
                }

                // Correlate prioritized and relevant keys to select a relevant key to place
                var prioritizedRelevantKeys = filteredKeys.Intersect(prioritizedItems);
                var selectedRelevantKey = prioritizedRelevantKeys.Any() ? pool.PeekAmong(prioritizedRelevantKeys, random) : pool.PeekAmong(filteredKeys, random);

                placeLog += $"Selected Key: {KeyManager.GetKey(selectedRelevantKey).Name}{Environment.NewLine}";

                // Filter out available locations where selected key cannot be placed
                var filteredLocations = randomizedLocations.Where(location => KeyAllowedInLocation(selectedRelevantKey, itemMap, pool, location.myRandomKeyIdentifier)).OrderBy(x => x.id).ToList();

                placeLog += $"Filtered Locations: {filteredLocations.Select(node => node.Name()).Aggregate((i, j) => i + ", " + j)}{Environment.NewLine}";

                pool.Pull(selectedRelevantKey);

                // Pick out one random accessible location, place the selected key there and add that item to inventory
                var relevantLocation = filteredLocations.ElementAt(random.Next(filteredLocations.Count));
                randomizedLocations.Remove(relevantLocation);
				itemMap.Add(relevantLocation.myRandomKeyIdentifier, selectedRelevantKey);
				inventory.myNodes.Add(relevantLocation);

                placeLog += $"Relevant Location: {relevantLocation.Name()}{Environment.NewLine}";

                prioritizedItems.Remove(selectedRelevantKey);

                // Only increase searchDepth if an actual item is placed (debatable if this is the correct approach)
                searchDepth++;

                UpdateLocationRestrictions(inventory, itemMap, pool, random);

                // Fill remaining accessible locations with random items
                foreach (var node in randomizedLocations)
				{
                    placeLog += $"Filling location: {node.Name()}{Environment.NewLine}";

                    // This is possible through Required Location Rules
                    if (itemMap.ContainsKey(node.myRandomKeyIdentifier))
                    {
                        placeLog += $"Location already filled with: {KeyManager.GetKey(itemMap[node.myRandomKeyIdentifier])?.Name ?? "Nothing"}{Environment.NewLine}";
                        inventory.myNodes.Add(node);
                        continue;
                    }

                    var filteredPrioritizedItems = prioritizedItems.Where(key => KeyAllowedInLocation(key, itemMap, pool, node.myRandomKeyIdentifier));

                    if (filteredPrioritizedItems.Any())
                    {
                        placeLog += $"Filtered Prioritized Items: {filteredPrioritizedItems.Select(key => KeyManager.GetKey(key)?.Name ?? "Nothing").Aggregate((i, j) => i + ", " + j)}{Environment.NewLine}";

                        var randomKey = pool.PullAmong(filteredPrioritizedItems, random);
                        prioritizedItems.Remove(randomKey);
                        itemMap.Add(node.myRandomKeyIdentifier, randomKey);
                        inventory.myNodes.Add(node);

                        placeLog += $"Prioritized Key placed: {KeyManager.GetKey(randomKey)?.Name ?? "Nothing"}{Environment.NewLine}";
                    }
                    else
                    {
                        // Add all items that cannot be in this location to restrictedItems
                        var locationRestrictedItems = restrictedItems.Union(pool.AvailableItems().Where(key => !KeyAllowedInLocation(key, itemMap, pool, node.myRandomKeyIdentifier)));

                        if (locationRestrictedItems.Any())
                        {
                            placeLog += $"Location Restricted Items: {locationRestrictedItems.Select(key => KeyManager.GetKey(key)?.Name ?? "Nothing").Aggregate((i, j) => i + ", " + j)}{Environment.NewLine}";
                        }

                        var selectedKey = pool.PullExcept(locationRestrictedItems, random);
                        
                        if (selectedKey != Guid.Empty)
                        {
                            itemMap.Add(node.myRandomKeyIdentifier, selectedKey);
                            inventory.myNodes.Add(node);

                            placeLog += $"Selected Key placed: {KeyManager.GetKey(selectedKey).Name}{Environment.NewLine}";
                        }
                    }

                    UpdateLocationRestrictions(inventory, itemMap, pool, random);
                }

				// Go back to start of loop to continue search with updated inventory
			}

            placeLog += $"{Environment.NewLine}Post-fill{Environment.NewLine}{Environment.NewLine}";

            // POST-FILL:
            // Reachable if seed is beatable or if it ran out of possible relevant keys
            // Fill in remaining locations as well as possible without breaking any restrictions
            
            // Do not place any power bombs until obtaining powered suit
            restrictedItems.Clear();
            if (options.noEarlyPbs && !inventory.ContainsKey(StaticKeys.CharlieDefeated))
            {
                restrictedItems.Add(StaticKeys.PowerBombs);
            }

            if (restrictedItems.Any())
            {
                placeLog += $"Restricted Items: {restrictedItems.Select(key => KeyManager.GetKey(key)?.Name ?? "Nothing").Aggregate((i, j) => i + ", " + j)}{Environment.NewLine}";
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

                UpdateLocationRestrictions(inventory, itemMap, pool, random);
            }

			return itemMap;
		}

        private void UpdateLocationRestrictions(Inventory inventory, Dictionary<string, Guid> itemMap, ItemPool pool, Random random)
        {
            placeLog += $"UpdateLocationRestrictions{Environment.NewLine}";

            bool requirementsUpdated = false;
            do
            {
                requirementsUpdated = false;

                var allRandomKeyNodes = keyNodes.Where(node => node is RandomKeyNode randomNode).Select(node => node as RandomKeyNode).OrderBy(node => node.id);
                var openRandomKeyNodes = allRandomKeyNodes.Where(node => !itemMap.ContainsKey(node.myRandomKeyIdentifier));

                var openRandomKeyNodeInAnyRequirement = openRandomKeyNodes.Where(loc => itemRequiredLocationRules.Any(group => group.Value.Contains(loc.myRandomKeyIdentifier)));

                if (openRandomKeyNodeInAnyRequirement.Any())
                {
                    placeLog += $"openRandomKeyNodeInAnyRequirement: {openRandomKeyNodeInAnyRequirement.Select(key => key.myRandomKeyIdentifier).Aggregate((i, j) => i + ", " + j)}{Environment.NewLine}";
                }

                foreach (var keyNode in openRandomKeyNodeInAnyRequirement)
                {
                    placeLog += $"Node: {keyNode.myRandomKeyIdentifier}{Environment.NewLine}";

                    var itemsWithOpenLocationRule = itemRequiredLocationRules.Where(group => !group.Value.Where(loc => itemMap.ContainsKey(loc) && itemMap[loc] == group.Key).Any());
                    var itemsWithFilteredOpenLocations = itemsWithOpenLocationRule.ToDictionary(group => group.Key, group => group.Value.Where(loc => !itemMap.ContainsKey(loc))).Where(item => item.Value.Any());
                    var itemsInPool = itemsWithFilteredOpenLocations.Where(item => pool.AvailableItems().Contains(item.Key)).ToList();

                    var relevantItems = itemsInPool.Where(group => group.Value.Any(loc => keyNode.myRandomKeyIdentifier == loc));

                    if (relevantItems.Any())
                    {
                        placeLog += $"RelevantItems: {relevantItems.Select(item => KeyManager.GetKey(item.Key)?.Name ?? "Nothing").Aggregate((i, j) => i + ", " + j)}{Environment.NewLine}";
                        var smallestNumberOfAvailableLocations = relevantItems.Select(item => item.Value.Count()).Min();

                        placeLog += $"smallestNumberOfAvailableLocations: {smallestNumberOfAvailableLocations}{Environment.NewLine}";
                        if (smallestNumberOfAvailableLocations <= relevantItems.Count())
                        {
                            placeLog += $"{keyNode.myRandomKeyIdentifier} restricted{Environment.NewLine}";
                            // Restrict location to only have rule items
                            restrictedLocations.Add(keyNode.myRandomKeyIdentifier);

                            if (smallestNumberOfAvailableLocations < relevantItems.Count())
                            {
                                var itemsOrderedByNumberOfAvailableLocations = relevantItems.OrderByDescending(item => item.Value.Count()).ThenBy(item => item.Key);

                                for (int i = 0; i < (relevantItems.Count() - smallestNumberOfAvailableLocations); i++)
                                {
                                    var itemId = itemsOrderedByNumberOfAvailableLocations.ElementAt(i).Key;
                                    var locationId = keyNode.myRandomKeyIdentifier;

                                    placeLog += $"{KeyManager.GetKey(itemId)?.Name ?? "Nothing"} removed from {locationId}{Environment.NewLine}";
                                    itemRequiredLocationRules[itemId].Remove(locationId);
                                    requirementsUpdated = true;
                                }
                            }

                            continue;
                        }
                    }

                    placeLog += $"{keyNode.myRandomKeyIdentifier} UNrestricted{Environment.NewLine}";
                    restrictedLocations.Remove(keyNode.myRandomKeyIdentifier);
                }
            } while (requirementsUpdated == true);

            if (restrictedLocations.Any())
            {
                placeLog += $"Restricted Locations: {restrictedLocations.Aggregate((i, j) => i + ", " + j)}{Environment.NewLine}";
            }
        }

        private bool KeyAllowedInLocation(Guid key, Dictionary<string, Guid> itemMap, ItemPool pool, string location)
        {
            if(itemBlockedLocationRules.ContainsKey(key) && itemBlockedLocationRules[key].Contains(location))
            {
                return false;
            }

            if(itemRequiredLocationRules.ContainsKey(key))
            {
                var requirementFulfilled = false;
                foreach(var loc in itemMap)
                {
                    if(itemRequiredLocationRules[key].Contains(loc.Key) && loc.Value == key)
                    {
                        requirementFulfilled = true;
                        break;
                    }
                }

                if (!requirementFulfilled && pool.CountKey(key) == 1 && !itemRequiredLocationRules[key].Contains(location))
                {
                    return false;
                }
            }

            if(restrictedLocations.Contains(location) && !(itemRequiredLocationRules.ContainsKey(key) && itemRequiredLocationRules[key].Contains(location)))
            {
                return false;
            }

            return true;
        }

        private void FillRandomly(List<Guid> restrictedItems, Inventory inventory, Dictionary<string, Guid> itemMap, ItemPool pool, Random random)
        {
            placeLog += $"Entering random fill{Environment.NewLine}";
            
            var reachableNodes = new List<NodeBase>();
            var localSearcher = new FillSearcher();

            // Fill all still reachable random nodes
            while (true)
            {
                placeLog += $"Step{Environment.NewLine}";

                reachableNodes.RemoveAll(node => inventory.myNodes.Contains(node));
                reachableNodes.AddRange(localSearcher.ContinueSearch(startNode, inventory, node => (node is KeyNode) && !inventory.myNodes.Contains(node)));

                if (reachableNodes.Any())
                {
                    placeLog += $"Reachable Nodes: {reachableNodes.Select(node => node.Name()).Aggregate((i, j) => i + ", " + j)}{Environment.NewLine}";
                }

                // Find all reachable events that are also possible to get back from
                var retracableEvents = reachableNodes.AsParallel().Where(node => node is EventKeyNode && NodeTraverser.PathExists(node, startNode, inventory.Expand(node))).ToList();

                // If any events can be reached, add to inventory and update search before continuing
                if (retracableEvents.Any())
                {
                    placeLog += $"Retracable Events: {retracableEvents.Select(node => node.Name()).Aggregate((i, j) => i + ", " + j)}{Environment.NewLine}";
                    inventory.myNodes.AddRange(retracableEvents);
                    continue;
                }

                var reachableRandomNodes = reachableNodes.Where(node => node is RandomKeyNode).Select(node => node as RandomKeyNode).OrderBy(x => x.id);

                var nodesToAdd = new List<NodeBase>();

                foreach (var node in reachableRandomNodes)
                {
                    placeLog += $"Filling location:: {node.Name()}{Environment.NewLine}";

                    if (itemMap.ContainsKey(node.myRandomKeyIdentifier))
                    {
                        placeLog += $"Location already filled with: {KeyManager.GetKey(itemMap[node.myRandomKeyIdentifier])?.Name ?? "Nothing"}{Environment.NewLine}";
                        nodesToAdd.Add(node);
                    }
                    else
                    {
                        // Add all items that cannot be in this location to restrictedItems
                        var locationRestrictedItems = restrictedItems.Union(pool.AvailableItems().Where(key => !KeyAllowedInLocation(key, itemMap, pool, node.myRandomKeyIdentifier)));

                        if (locationRestrictedItems.Any())
                        {
                            placeLog += $"Location Restricted Items: {locationRestrictedItems.Select(key => KeyManager.GetKey(key)?.Name ?? "Nothing").Aggregate((i, j) => i + ", " + j)}{Environment.NewLine}";
                        }

                        if (!pool.AvailableItems().Except(locationRestrictedItems).Any())
                            return;

                        var selectedKey = pool.PullExcept(locationRestrictedItems, random);
                        if (selectedKey != Guid.Empty)
                        {
                            placeLog += $"Selected Key: {KeyManager.GetKey(selectedKey)?.Name ?? "Nothing"}{Environment.NewLine}";
                            itemMap.Add(node.myRandomKeyIdentifier, selectedKey);
                            nodesToAdd.Add(node);
                        }

                        UpdateLocationRestrictions(inventory, itemMap, pool, random);
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
