using Common.Key;
using Common.Node;
using Common.SaveData;
using Randomizer.ItemRules;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public LogLayer Log { get; private set; } = new LogLayer();

		public Dictionary<string, Guid> FillLocations(SaveData someData, FillOptions options)
		{
			var pool = new ItemPool();
			pool.CreatePool();

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
			pool.CreatePool();

			return FillLocations(someData, options, pool, startingInventory);
		}

		public Dictionary<string, Guid> FillLocations(SaveData someData, FillOptions options, Random random)
		{
			var pool = new ItemPool();
			pool.CreatePool();

			return FillLocations(someData, options, pool, random);
		}

        public Dictionary<string, Guid> FillLocations(SaveData someData, FillOptions options, Dictionary<string, Guid> itemMap)
        {
            var pool = new ItemPool();
            pool.CreatePool();

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
			pool.CreatePool();

			return FillLocations(someData, options, pool, startingInventory, random);
		}

        public Dictionary<string, Guid> FillLocations(SaveData someData, FillOptions options, Inventory startingInventory, Dictionary<string, Guid> itemMap)
        {
            var pool = new ItemPool();
            pool.CreatePool();

            return FillLocations(someData, options, pool, startingInventory, itemMap);
        }

        public Dictionary<string, Guid> FillLocations(SaveData someData, FillOptions options, Random random, Dictionary<string, Guid> itemMap)
        {
            var pool = new ItemPool();
            pool.CreatePool();

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
            Log = new LogLayer("Item Placement");

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
                        Log.AddChild($"Overlapping Blocked Required Rules for item {KeyManager.GetKeyName(rule.Key)}", overlappingLocations);

                        itemRequiredLocationRules[rule.Key].RemoveAll(loc => overlappingLocations.Contains(loc));
                    }

                    if(!itemRequiredLocationRules[rule.Key].Any())
                    {
                        Log.AddChild($"Item {KeyManager.GetKeyName(rule.Key)} left with no requirement rules, removed");
                        itemRequiredLocationRules.Remove(rule.Key);
                    }
                }
            }

            if (itemRequiredLocationRules.Any())
            {
                Log.AddChild("RequiredLocationRules", itemRequiredLocationRules.Select(group => new LogLayer(KeyManager.GetKeyName(group.Key), group.Value)));
            }

            if (itemBlockedLocationRules.Any())
            {
                Log.AddChild("BlockedLocationRules", itemBlockedLocationRules.Select(group => new LogLayer(KeyManager.GetKeyName(group.Key), group.Value)));
            }

            UpdateLocationRestrictions(inventory, itemMap, pool, random, Log);

            searcher = new FillSearcher();

			var reachableKeys = new List<NodeBase>();
			var retracableKeys = new List<NodeBase>();
            var restrictedItems = new List<Guid>();

            var searchDepth = 0;
            var stepCount = 1;

            var logSteps = Log.AddChild("Standard Steps");
            while(true)
			{
                var logCurrentStep = logSteps.AddChild($"Step {stepCount++}, depth {searchDepth}");
                logCurrentStep.AddChild(inventory.GetKeyLog());

                // Beatable conditional
                if (options.gameCompletion == FillOptions.GameCompletion.Beatable && inventory.myNodes.Contains(endNode))
                {
                    logCurrentStep.AddChild($"EndNode reached");
                    break;
                }

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
                    logCurrentStep.AddChild("Restricted Items", restrictedItems.Select(key => KeyManager.GetKeyName(key)));
                }

                // Find all nodes that can be reached with current inventory
				reachableKeys.RemoveAll(node => inventory.myNodes.Contains(node));
				reachableKeys.AddRange(searcher.ContinueSearch(startNode, inventory, node => (node is KeyNode) && !inventory.myNodes.Contains(node)));

                logCurrentStep.AddChild("Reachable keys", reachableKeys.Select(node => node.Name()));

                // Special case to handle how chozodia area is built 
                // Specifically can't get out of it without power bombs, which creates awkward dynamics regarding the placements of said power bombs)
                // Special case triggers on reaching charlie without picking up any power bombs
                if (reachableKeys.Any(key => key is EventKeyNode eventKey && eventKey.myKeyId == StaticKeys.CharlieDefeated) &&
					!inventory.ContainsKey(StaticKeys.PowerBombs))
				{
                    logCurrentStep.AddChild("Charlie special condition");

                    FillRandomly(restrictedItems, inventory, itemMap, pool, random, logCurrentStep);

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

                logCurrentStep.AddChild("Retracable keys", retracableKeys.Select(node => node.Name()));

                if (!retracableKeys.Any())
                {
                    break;
                }

                // If any events can be reached, add to inventory and update search before continuing
                var retracableEvents = retracableKeys.Where(node => node is EventKeyNode).ToList();
                if (retracableEvents.Any())
				{
                    logCurrentStep.AddChild("Retracable events", retracableEvents.Select(node => node.Name()));
                    inventory.myNodes.AddRange(retracableEvents);
					continue;
				}

                var randomizedLocations = retracableKeys.Where(key => key is RandomKeyNode randomNode).Select(key => key as RandomKeyNode).OrderBy(x => x.id).ToList();
                var randomizedLocationNames = randomizedLocations.Select(loc => loc.myRandomKeyIdentifier);

                // Pick up any items already filled in on the map and update search before placing any items
                var preFilledLocations = randomizedLocations.Where(loc => loc.GetKey() != null);

                if (preFilledLocations.Any())
                {
                    logCurrentStep.AddChild("Prefilled locations", preFilledLocations.Select(node => node.Name()));
                    inventory.myNodes.AddRange(preFilledLocations);
                    continue;
                }

                // Find which possible keys would expand number of retracable nodes
                var relevantKeys = FindRelevantKeys(inventory, restrictedItems, reachableKeys.Except(retracableKeys), pool);

                logCurrentStep.AddChild("Relevant Keys", relevantKeys.Select(key => KeyManager.GetKeyName(key)));

                // Filter out any keys that by rules cannot be placed in any available location
                var filteredKeys = relevantKeys.Where(key => randomizedLocationNames.Any(location => KeyAllowedInLocation(key, itemMap, pool, location)));

                logCurrentStep.AddChild("FilteredKeys", filteredKeys.Select(key => KeyManager.GetKeyName(key)));
                if (!filteredKeys.Any())
                {
                    break;
                }

                // Get items that are prioritized according to item rules
                var prioritizedItems = options.itemRules.Where(rest => rest is ItemRulePrioritizedAfterDepth depthRest && depthRest.SearchDepth <= searchDepth)
                    .Select(rest => rest.ItemId)
                    .Where(item => !restrictedItems.Contains(item) && !inventory.ContainsKey(item))
                    .ToList();

                logCurrentStep.AddChild("Prioritized Items", prioritizedItems.Select(key => KeyManager.GetKeyName(key)));

                // Correlate prioritized and relevant keys to select a relevant key to place
                var prioritizedRelevantKeys = filteredKeys.Intersect(prioritizedItems);
                var selectedRelevantKey = prioritizedRelevantKeys.Any() ? pool.PeekAmong(prioritizedRelevantKeys, random) : pool.PeekAmong(filteredKeys, random);

                logCurrentStep.AddChild($"Selected Key: {KeyManager.GetKeyName(selectedRelevantKey)}");

                // Filter out available locations where selected key cannot be placed
                var filteredLocations = randomizedLocations.Where(location => KeyAllowedInLocation(selectedRelevantKey, itemMap, pool, location.myRandomKeyIdentifier)).OrderBy(x => x.id).ToList();

                logCurrentStep.AddChild("Filtered Locations", filteredLocations.Select(node => node.Name()));

                pool.Pull(selectedRelevantKey);

                // Pick out one random accessible location, place the selected key there and add that item to inventory
                var selectedLocation = filteredLocations.ElementAt(random.Next(filteredLocations.Count));
                randomizedLocations.Remove(selectedLocation);
				itemMap.Add(selectedLocation.myRandomKeyIdentifier, selectedRelevantKey);
				inventory.myNodes.Add(selectedLocation);

                logCurrentStep.AddChild($"Selected Location: {selectedLocation.Name()}");

                prioritizedItems.Remove(selectedRelevantKey);

                // Only increase searchDepth if an actual item is placed (debatable if this is the correct approach)
                searchDepth++;

                UpdateLocationRestrictions(inventory, itemMap, pool, random, logCurrentStep);

                var randomizedLocationLog = logCurrentStep.AddChild("Randomized Locations");
                // Fill remaining accessible locations with random items
                foreach (var node in randomizedLocations)
				{
                    var locationLog = randomizedLocationLog.AddChild(node.Name());

                    // This is possible through Required Location Rules
                    if (itemMap.ContainsKey(node.myRandomKeyIdentifier))
                    {
                        locationLog.AddChild($"Already filled with: {KeyManager.GetKeyName(itemMap[node.myRandomKeyIdentifier])}");
                        inventory.myNodes.Add(node);
                        continue;
                    }

                    var filteredPrioritizedItems = prioritizedItems.Where(key => KeyAllowedInLocation(key, itemMap, pool, node.myRandomKeyIdentifier));

                    if (filteredPrioritizedItems.Any())
                    {
                        locationLog.AddChild("Filtered Prioritized Items", filteredPrioritizedItems.Select(key => KeyManager.GetKeyName(key)));

                        var randomKey = pool.PullAmong(filteredPrioritizedItems, random);
                        prioritizedItems.Remove(randomKey);
                        itemMap.Add(node.myRandomKeyIdentifier, randomKey);
                        inventory.myNodes.Add(node);

                        locationLog.Message += $" : {KeyManager.GetKeyName(randomKey)}";
                        locationLog.AddChild($"Prioritized Key placed: {KeyManager.GetKeyName(randomKey)}");
                    }
                    else
                    {
                        // Add all items that cannot be in this location to restrictedItems
                        var locationRestrictedItems = restrictedItems.Union(pool.AvailableItems().Where(key => !KeyAllowedInLocation(key, itemMap, pool, node.myRandomKeyIdentifier)));

                        locationLog.AddChild("Location Restricted Items", locationRestrictedItems.Select(key => KeyManager.GetKeyName(key)));

                        var selectedKey = pool.PullExcept(locationRestrictedItems, random);
                        
                        if (selectedKey != Guid.Empty)
                        {
                            itemMap.Add(node.myRandomKeyIdentifier, selectedKey);
                            inventory.myNodes.Add(node);

                            locationLog.Message += $" : {KeyManager.GetKeyName(selectedKey)}";
                            locationLog.AddChild($"Selected Key placed: {KeyManager.GetKeyName(selectedKey)}");
                        }
                    }

                    UpdateLocationRestrictions(inventory, itemMap, pool, random, locationLog);
                }

				// Go back to start of loop to continue search with updated inventory
			}

            var postFillLog = Log.AddChild("Post fill");

            // POST-FILL:
            // Reachable if seed is beatable or if it ran out of possible relevant keys
            // Fill in remaining locations as well as possible without breaking any restrictions
            
            // Do not place any power bombs until obtaining powered suit
            restrictedItems.Clear();
            if (options.noEarlyPbs && !inventory.ContainsKey(StaticKeys.CharlieDefeated))
            {
                restrictedItems.Add(StaticKeys.PowerBombs);
            }

            postFillLog.AddChild("Restricted Items", restrictedItems.Select(key => KeyManager.GetKeyName(key)));

            if (options.gameCompletion == FillOptions.GameCompletion.AllItems)
			{
                // Prioritize filling in non-empty items
                var restrictedAndEmpty = new List<Guid>(restrictedItems);
                restrictedAndEmpty.Add(StaticKeys.Nothing);

                FillRandomly(restrictedAndEmpty, inventory, itemMap, pool, random, postFillLog);
            }

            // Fill remaining reachable nodes randomly
            FillRandomly(restrictedItems, inventory, itemMap, pool, random, postFillLog);

            // Fill all remaining (unreachable) locations with any remaining items
            var remainingNodes = keyNodes.AsParallel().Where(node => node is RandomKeyNode randomNode && !itemMap.ContainsKey(randomNode.myRandomKeyIdentifier)).Select(key => key as RandomKeyNode).OrderBy(x => x.id).ToList();

            var finalFill = postFillLog.AddChild("Final fill");
			foreach (var node in remainingNodes)
			{
                if (itemMap.ContainsKey(node.myRandomKeyIdentifier))
                {
                    continue;
                }

                var locationLog = finalFill.AddChild(node.myRandomKeyIdentifier);

                var item = pool.Pull(random);

                locationLog.Message += $" : {KeyManager.GetKeyName(item)}";
                locationLog.AddChild($"Selected item: {KeyManager.GetKeyName(item)}");

                itemMap.Add(node.myRandomKeyIdentifier, item);

                UpdateLocationRestrictions(inventory, itemMap, pool, random, locationLog);
            }

			return itemMap;
		}

        private void UpdateLocationRestrictions(Inventory inventory, Dictionary<string, Guid> itemMap, ItemPool pool, Random random, LogLayer log)
        {
            var localLog = log.AddChild("UpdateLocationRestrictions");
            localLog.AddChild(inventory.GetKeyLog());

            bool requirementsUpdated = false;
            do
            {
                requirementsUpdated = false;

                var allRandomKeyNodes = keyNodes.Where(node => node is RandomKeyNode randomNode).Select(node => node as RandomKeyNode).OrderBy(node => node.id);
                var openRandomKeyNodes = allRandomKeyNodes.Where(node => !itemMap.ContainsKey(node.myRandomKeyIdentifier));

                var openRandomKeyNodeInAnyRequirement = openRandomKeyNodes.Where(loc => itemRequiredLocationRules.Any(group => group.Value.Contains(loc.myRandomKeyIdentifier)));

                var openKeysLog = localLog.AddChild("Available RandomKeyNodes in a Requirement");

                foreach (var keyNode in openRandomKeyNodeInAnyRequirement)
                {
                    var locationLog = openKeysLog.AddChild(keyNode.myRandomKeyIdentifier);

                    var itemsWithOpenLocationRule = itemRequiredLocationRules.Where(group => !group.Value.Any(loc => itemMap.ContainsKey(loc) && itemMap[loc] == group.Key));
                    var itemsWithFilteredOpenLocations = itemsWithOpenLocationRule.ToDictionary(group => group.Key, group => group.Value.Where(loc => !itemMap.ContainsKey(loc))).Where(item => item.Value.Any());
                    var itemsInPool = itemsWithFilteredOpenLocations.Where(item => pool.AvailableItems().Contains(item.Key)).ToList();

                    var relevantItems = itemsInPool.Where(group => group.Value.Any(loc => keyNode.myRandomKeyIdentifier == loc));

                    if (relevantItems.Any())
                    {
                        locationLog.AddChild("RelevantItems", relevantItems.Select(item => KeyManager.GetKeyName(item.Key)));

                        var itemsOrderedByNumberOfAvailableLocations = relevantItems.OrderByDescending(item => item.Value.Count()).ThenBy(item => item.Key);

                        locationLog.AddChild("Items Ordered By Number Of Available Locations", itemsOrderedByNumberOfAvailableLocations.Select(item => $"{KeyManager.GetKeyName(item.Key)} - {item.Value.Count()}"));

                        var smallestNumberOfAvailableLocations = itemsOrderedByNumberOfAvailableLocations.Last().Value.Count();

                        locationLog.AddChild($"Smallest Number Of Available Locations: {smallestNumberOfAvailableLocations}");

                        if (smallestNumberOfAvailableLocations <= relevantItems.Count())
                        {
                            locationLog.AddChild("Location restricted");

                            // Restrict location to only have rule items
                            restrictedLocations.Add(keyNode.myRandomKeyIdentifier);

                            if (smallestNumberOfAvailableLocations < relevantItems.Count())
                            {
                                var removedLog = locationLog.AddChild("Removed Items");

                                for (int i = 0; i < (relevantItems.Count() - smallestNumberOfAvailableLocations); i++)
                                {
                                    var itemId = itemsOrderedByNumberOfAvailableLocations.ElementAt(i).Key;
                                    var locationId = keyNode.myRandomKeyIdentifier;

                                    removedLog.AddChild(KeyManager.GetKeyName(itemId));

                                    itemRequiredLocationRules[itemId].Remove(locationId);
                                    requirementsUpdated = true;
                                }
                            }

                            continue;
                        }
                    }

                    locationLog.AddChild("Unrestricted");
                    restrictedLocations.Remove(keyNode.myRandomKeyIdentifier);
                }

            } while (requirementsUpdated == true);

            localLog.AddChild(new LogLayer("Restricted Locations", restrictedLocations));
        }

        private bool KeyAllowedInLocation(Guid key, Dictionary<string, Guid> itemMap, ItemPool pool, string location)
        {
            if(itemBlockedLocationRules.ContainsKey(key) && itemBlockedLocationRules[key].Contains(location))
            {
                // Blocked from being placed here
                return false;
            }

            var hasRequirementForLocation = false;
            var requirementFulfilled = true;
            if (itemRequiredLocationRules.ContainsKey(key))
            {
                requirementFulfilled = itemMap.Any(loc => itemRequiredLocationRules[key].Contains(loc.Key) && loc.Value == key);

                hasRequirementForLocation = itemRequiredLocationRules[key].Contains(location);
            }

            if (!requirementFulfilled && pool.CountKey(key) == 1 && !hasRequirementForLocation)
            {
                // Has requirement to be placed somewhere else and placing it here would not leave any item to place in that location
                return false;
            }

            if (restrictedLocations.Contains(location) && (requirementFulfilled || !hasRequirementForLocation))
            {
                // Location is restricted and this key is not one of the items with a requirement to be here
                return false;
            }

            return true;
        }

        private void FillRandomly(List<Guid> restrictedItems, Inventory inventory, Dictionary<string, Guid> itemMap, ItemPool pool, Random random, LogLayer log)
        {
            var fillLog = log.AddChild("Random Fill");
            fillLog.AddChild("Restricted Items", restrictedItems.Select(key => KeyManager.GetKeyName(key)));
            
            var reachableNodes = new List<NodeBase>();
            var localSearcher = new FillSearcher();

            // Fill all still reachable random nodes
            int stepCount = 1;
            while (true)
            {
                var stepLog = new LogLayer($"Step {stepCount++}");
                stepLog.AddChild(inventory.GetKeyLog());

                reachableNodes.RemoveAll(node => inventory.myNodes.Contains(node));
                reachableNodes.AddRange(localSearcher.ContinueSearch(startNode, inventory, node => (node is KeyNode) && !inventory.myNodes.Contains(node)));

                stepLog.AddChild(new LogLayer("Reachable Nodes", reachableNodes.Select(node => node.Name())));

                var retracableKeys = reachableNodes.AsParallel().Where(node => node == endNode || NodeTraverser.PathExists(node, startNode, node is EventKeyNode ? inventory.Expand(node) : inventory)).ToList();

                // Find all reachable events that are also possible to get back from
                var retracableEvents = retracableKeys.Where(node => node is EventKeyNode);

                // If any events can be reached, add to inventory and update search before continuing
                if (retracableEvents.Any())
                {
                    stepLog.AddChild("Retracable events", retracableEvents.Select(node => node.Name()));
                    fillLog.AddChild(stepLog);
                    inventory.myNodes.AddRange(retracableEvents);
                    continue;
                }

                var fillNodes = retracableKeys.Any() ? retracableKeys : reachableNodes;

                var fillRandomKeyNodes = fillNodes.Where(node => node is RandomKeyNode).Select(node => node as RandomKeyNode).OrderBy(x => x.id);

                var nodesToAdd = new List<NodeBase>();

                var locationsLog = stepLog.AddChild("Locations to fill");
                foreach (var node in fillRandomKeyNodes)
                {
                    var locationLog = locationsLog.AddChild(node.Name());
                    
                    if (itemMap.ContainsKey(node.myRandomKeyIdentifier))
                    {
                        locationLog.AddChild($"Location already filled with: {KeyManager.GetKeyName(itemMap[node.myRandomKeyIdentifier])}");
                        nodesToAdd.Add(node);
                    }
                    else
                    {
                        // Add all items that cannot be in this location to restrictedItems
                        var locationRestrictedItems = restrictedItems.Union(pool.AvailableItems().Where(key => !KeyAllowedInLocation(key, itemMap, pool, node.myRandomKeyIdentifier)));

                        locationLog.AddChild(new LogLayer("Location Restricted Items", locationRestrictedItems.Select(key => KeyManager.GetKeyName(key))));

                        if (!pool.AvailableItems().Except(locationRestrictedItems).Any())
                        {
                            locationLog.Message += $" : Skipped";
                            locationLog.AddChild("No available item is allowed to be placed");
                            continue;
                        }

                        var selectedKey = pool.PullExcept(locationRestrictedItems, random);
                        if (selectedKey != Guid.Empty)
                        {
                            locationLog.Message += $" : {KeyManager.GetKeyName(selectedKey)}";
                            locationLog.AddChild($"Selected Key: {KeyManager.GetKeyName(selectedKey)}");
                            itemMap.Add(node.myRandomKeyIdentifier, selectedKey);
                            nodesToAdd.Add(node);
                        }

                        UpdateLocationRestrictions(inventory, itemMap, pool, random, locationLog);
                    }
                }

                if (!nodesToAdd.Any())
                    break;

                fillLog.AddChild(stepLog);
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
