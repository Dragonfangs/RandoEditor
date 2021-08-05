using Common.Key;
using Common.Node;
using Common.SaveData;
using Verifier.ItemRules;
using System;
using System.Collections.Generic;
using System.Linq;
using Verifier;
using Verifier.Key;
using Common.Log;

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
        private EventKeyNode charlieNode = null;

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

        public Dictionary<string, Guid> FillLocations(SaveData someData, FillOptions options, ItemPool pool, Inventory startingInventory, Random random, Dictionary<string, Guid> originalItemMap)
		{
            Log = new LogLayer("Item Placement");

            var inventory = new Inventory(startingInventory);

            var nodeCollection = new NodeCollection();
            nodeCollection.InitializeNodes(someData);

            keyNodes = nodeCollection.myNodes.Where(node => node is KeyNode).ToList();
            var eventNodes = keyNodes.Where(node => node is EventKeyNode).Select(node => node as EventKeyNode);
            startNode = eventNodes.FirstOrDefault(node => node.myKeyId == StaticKeys.GameStart);
            endNode = eventNodes.FirstOrDefault(node => node.myKeyId == StaticKeys.GameFinish);
            charlieNode = eventNodes.FirstOrDefault(node => node.myKeyId == StaticKeys.CharlieDefeated);

            // Generate Item map
            var itemMap = new Dictionary<string, Guid>(originalItemMap);

            if (options.majorSwap == FillOptions.ItemSwap.Unchanged || options.minorSwap == FillOptions.ItemSwap.Unchanged)
            {
                var allRandomKeyNodes = keyNodes.Where(node => node is RandomKeyNode randomNode).Select(node => node as RandomKeyNode).OrderBy(node => node.id);
                var openRandomKeyNodes = allRandomKeyNodes.Where(node => !itemMap.ContainsKey(node.myRandomKeyIdentifier));

                foreach (var keyNode in openRandomKeyNodes)
                {
                    var item = keyNode.GetOriginalKey();
                    if (item == null)
                        continue;

                    if ((StaticKeys.IsMajorItem(item.Id) && options.majorSwap == FillOptions.ItemSwap.Unchanged) ||
                        (StaticKeys.IsMinorItem(item.Id) && options.minorSwap == FillOptions.ItemSwap.Unchanged))
                    {
                        var pulled = pool.Pull(item.Id);
                        if (pulled)
                        {
                            itemMap.Add(keyNode.myRandomKeyIdentifier, item.Id);
                        }
                    }
                }
            }

            KeyManager.SetRandomKeyMap(itemMap);

            // Set up location rules
            itemRequiredLocationRules = ItemRuleUtility.GetRequiredLocationRules(options.itemRules);
            itemBlockedLocationRules = ItemRuleUtility.GetBlockedLocationRules(options.itemRules);

            restrictedLocations = new HashSet<string>();

            if (itemRequiredLocationRules.Any())
            {
                Log.AddChild("RequiredLocationRules", itemRequiredLocationRules.Select(group => new LogLayer(KeyManager.GetKeyName(group.Key), group.Value)));
            }

            if (itemBlockedLocationRules.Any())
            {
                Log.AddChild("BlockedLocationRules", itemBlockedLocationRules.Select(group => new LogLayer(KeyManager.GetKeyName(group.Key), group.Value)));
            }

            UpdateLocationRestrictions(inventory, itemMap, pool, random, Log);

            // Initialize search terms
            searcher = new FillSearcher();

			var reachableKeys = new List<NodeBase>();
			var retracableKeys = new List<NodeBase>();
            var restrictedItems = new List<Guid>();

            var searchDepth = 0;
            var stepCount = 1;

            if (options.gameCompletion == FillOptions.GameCompletion.NoLogic)
            {
                Log.AddChild("Game Completion has No Logic - Skipping standard steps");
            }
            else
            {
                var logSteps = Log.AddChild("Standard Steps");
                while (true)
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

                    // Pick up any items already filled in on the map and update search before placing any items
                    var preFilledLocations = randomizedLocations.Where(loc => itemMap.ContainsKey(loc.myRandomKeyIdentifier));

                    if (preFilledLocations.Any())
                    {
                        logCurrentStep.AddChild("Prefilled locations", preFilledLocations.Select(node => $"{node.Name()} - {node.GetKeyName()}"));
                        inventory.myNodes.AddRange(preFilledLocations);
                        continue;
                    }

                    // Get items that are prioritized according to item rules
                    var prioritizedItems = options.itemRules.Where(rest => rest is ItemRulePrioritizedAfterDepth depthRest && depthRest.SearchDepth <= searchDepth)
                        .Select(rest => rest.ItemId)
                        .Where(item => !restrictedItems.Contains(item) && !inventory.ContainsKey(item))
                        .ToList();

                    logCurrentStep.AddChild("Prioritized Items", prioritizedItems.Select(key => KeyManager.GetKeyName(key)));

                    var selectedRelevantKey = FindRelevantKey(inventory, searcher, options, randomizedLocations, reachableKeys.Except(retracableKeys), restrictedItems, prioritizedItems, pool, itemMap, random, logCurrentStep);
                    
                    // Special case to handle how chozodia area is built 
                    // Specifically can't get out of it without power bombs, which creates awkward dynamics regarding the placements of said power bombs)
                    // Special case triggers on reaching charlie when no early pbs is enabled
                    if (reachableKeys.Any(key => key is EventKeyNode eventKey && eventKey.myKeyId == StaticKeys.CharlieDefeated) && options.noEarlyPbs &&
                        (NodeTraverser.PathExists(charlieNode, endNode, inventory.Expand(charlieNode)) || selectedRelevantKey == Guid.Empty))
                    {
                        FillRandomly(restrictedItems, inventory, options, itemMap, pool, random, logCurrentStep.AddChild("Charlie random fill"));

                        if (!inventory.ContainsKey(StaticKeys.CharlieDefeated))
                        {
                            // Unless Charlie was for some reason reached during fill, start new search with Charlie as new start node
                            startNode = charlieNode;
                            inventory.myNodes.Add(startNode);
                            searcher = new FillSearcher();
                        }

                        continue;
                    }

                    if (selectedRelevantKey == Guid.Empty)
                    {
                        break;
                    }

                    // Filter out available locations where selected key cannot be placed
                    var filteredLocations = randomizedLocations.Where(location => KeyAllowedInLocation(selectedRelevantKey, options, itemMap, pool, location)).OrderBy(x => x.id).ToList();

                    logCurrentStep.AddChild("Filtered Locations", filteredLocations.Select(node => node.Name()));

                    if (!filteredLocations.Any())
                    {
                        break;
                    }

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

                    // Fill remaining accessible locations with random items
                    if (options.majorSwap != FillOptions.ItemSwap.LocalPool && options.minorSwap != FillOptions.ItemSwap.LocalPool)
                    {
                        var randomizedLocationLog = logCurrentStep.AddChild("Randomized Locations");
                        foreach (var node in randomizedLocations)
                        {
                            var locationLog = randomizedLocationLog.AddChild(node.Name());

                            // This is possible through Required Location Rules
                            if (itemMap.ContainsKey(node.myRandomKeyIdentifier))
                            {
                                locationLog.AddChild($"Already filled with: {node.GetKeyName()}");
                                inventory.myNodes.Add(node);
                                continue;
                            }

                            var filteredPrioritizedItems = prioritizedItems.Where(key => KeyAllowedInLocation(key, options, itemMap, pool, node));

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
                                var locationRestrictedItems = restrictedItems.Union(pool.AvailableItems().Where(key => !KeyAllowedInLocation(key, options, itemMap, pool, node)));

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
                    }

                    // Go back to start of loop to continue search with updated inventory
                }
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
                var nonEmptyLog = postFillLog.AddChild("Add non-empty items");

                // Prioritize filling in non-empty items
                var restrictedAndEmpty = new List<Guid>(restrictedItems);
                restrictedAndEmpty.Add(StaticKeys.Nothing);

                FillRandomly(restrictedAndEmpty, inventory, options, itemMap, pool, random, nonEmptyLog);
            }

            var reachableLog = postFillLog.AddChild("Fill remaining reachable locations");

            // Fill remaining reachable nodes randomly
            FillRandomly(restrictedItems, inventory, options, itemMap, pool, random, reachableLog);

            restrictedItems.Clear();

            // Fill all remaining (unreachable) locations with any remaining items
            var remainingNodes = keyNodes.Where(node => node is RandomKeyNode randomNode && !itemMap.ContainsKey(randomNode.myRandomKeyIdentifier)).Select(key => key as RandomKeyNode).OrderBy(x => x.id);

            if (remainingNodes.Any())
            {
                var respectableLog = postFillLog.AddChild("Fill unreachable locations");

                FillRandomly(remainingNodes, restrictedItems, inventory, options, itemMap, pool, random, respectableLog);
            }

            // Fill final locations with blanks
            var finalEmptyLocations = keyNodes.Where(node => node is RandomKeyNode randomNode && !itemMap.ContainsKey(randomNode.myRandomKeyIdentifier)).Select(key => key as RandomKeyNode).OrderBy(x => x.id);

            if (finalEmptyLocations.Any())
            {
                var finalFill = postFillLog.AddChild("Final fill");
                foreach (var node in finalEmptyLocations)
                {
                    if (itemMap.ContainsKey(node.myRandomKeyIdentifier))
                    {
                        continue;
                    }

                    var locationLog = finalFill.AddChild(node.myRandomKeyIdentifier);

                    var item = StaticKeys.Nothing;

                    locationLog.Message += $" : {KeyManager.GetKeyName(item)}";
                    locationLog.AddChild($"Selected item: {KeyManager.GetKeyName(item)}");

                    itemMap.Add(node.myRandomKeyIdentifier, item);

                    UpdateLocationRestrictions(inventory, itemMap, pool, random, locationLog);
                }
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

        private bool KeyAllowedInLocation(Guid key, FillOptions options, Dictionary<string, Guid> itemMap, ItemPool pool, RandomKeyNode location)
        {
            if(itemBlockedLocationRules.ContainsKey(key) && itemBlockedLocationRules[key].Contains(location.myRandomKeyIdentifier))
            {
                // Blocked from being placed here
                return false;
            }

            if ((StaticKeys.IsMajorItem(key) && options.majorSwap == FillOptions.ItemSwap.Unchanged) ||
                (StaticKeys.IsMinorItem(key) && options.minorSwap == FillOptions.ItemSwap.Unchanged))
            {
                if (key != location.myOriginalKeyId)
                    return false;
            }

            if (options.majorSwap == FillOptions.ItemSwap.LocalPool && StaticKeys.IsMajorItem(key) && !StaticKeys.IsMajorItem(location.myOriginalKeyId))
                return false;

            if (options.minorSwap == FillOptions.ItemSwap.LocalPool && StaticKeys.IsMinorItem(key) && !StaticKeys.IsMinorItem(location.myOriginalKeyId))
                return false;

            if (options.majorSwap == FillOptions.ItemSwap.LocalPool && !StaticKeys.IsMajorItem(key) && StaticKeys.IsMajorItem(location.myOriginalKeyId))
            {
                var allRandomKeyNodes = keyNodes.Where(node => node is RandomKeyNode randomNode).Select(node => node as RandomKeyNode).OrderBy(node => node.id);
                var openMajorRandomKeyNodes = allRandomKeyNodes.Count(node => !itemMap.ContainsKey(node.myRandomKeyIdentifier) && StaticKeys.IsMajorItem(node.myOriginalKeyId));

                var poolCount = pool.AvailableItems().Count(item => StaticKeys.IsMajorItem(item));
                if (openMajorRandomKeyNodes <= poolCount)
                    return false;
            }

            if (options.minorSwap == FillOptions.ItemSwap.LocalPool && !StaticKeys.IsMinorItem(key) && StaticKeys.IsMinorItem(location.myOriginalKeyId))
            {
                var allRandomKeyNodes = keyNodes.Where(node => node is RandomKeyNode randomNode).Select(node => node as RandomKeyNode).OrderBy(node => node.id);
                var openMinorRandomKeyNodes = allRandomKeyNodes.Count(node => !itemMap.ContainsKey(node.myRandomKeyIdentifier) && StaticKeys.IsMinorItem(node.myOriginalKeyId));

                var poolCount = pool.AvailableItems().Count(item => StaticKeys.IsMinorItem(item));
                if (openMinorRandomKeyNodes <= poolCount)
                    return false;
            }

            var hasRequirementForLocation = false;
            var requirementFulfilled = true;
            if (itemRequiredLocationRules.ContainsKey(key))
            {
                requirementFulfilled = itemMap.Any(loc => itemRequiredLocationRules[key].Contains(loc.Key) && loc.Value == key);

                hasRequirementForLocation = itemRequiredLocationRules[key].Contains(location.myRandomKeyIdentifier);
            }

            if (!requirementFulfilled && pool.CountKey(key) == 1 && !hasRequirementForLocation)
            {
                // Has requirement to be placed somewhere else and placing it here would not leave any item to place in that location
                return false;
            }

            if (restrictedLocations.Contains(location.myRandomKeyIdentifier) && (requirementFulfilled || !hasRequirementForLocation))
            {
                // Location is restricted and this key is not one of the items with a requirement to be here
                return false;
            }

            return true;
        }

        private void FillRandomly(List<Guid> restrictedItems, 
            Inventory inventory, FillOptions options, Dictionary<string, Guid> itemMap, ItemPool pool, 
            Random random, LogLayer log)
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

                var addedNodes = FillRandomly(fillRandomKeyNodes, restrictedItems, inventory, options, itemMap, pool, random, stepLog);

                if (!addedNodes)
                    break;

                fillLog.AddChild(stepLog);
            }
        }

        private bool FillRandomly(IOrderedEnumerable<RandomKeyNode> locations, List<Guid> restrictedItems, 
            Inventory inventory, FillOptions options, Dictionary<string, Guid> itemMap, ItemPool pool,
            Random random, LogLayer log)
        {
            var addedNodes = false;

            var locationsLog = log.AddChild("Locations to fill");
            foreach (var node in locations)
            {
                var locationLog = locationsLog.AddChild(node.Name());

                if (itemMap.ContainsKey(node.myRandomKeyIdentifier))
                {
                    locationLog.AddChild($"Location already filled with: {KeyManager.GetKeyName(itemMap[node.myRandomKeyIdentifier])}");
                    inventory.myNodes.Add(node);

                    addedNodes = true;
                }
                else
                {
                    // Add all items that cannot be in this location to restrictedItems
                    var locationRestrictedItems = restrictedItems.Union(pool.AvailableItems().Where(key => !KeyAllowedInLocation(key, options, itemMap, pool, node)));

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
                        inventory.myNodes.Add(node);

                        addedNodes = true;
                    }

                    UpdateLocationRestrictions(inventory, itemMap, pool, random, locationLog);
                }
            }

            return addedNodes;
        }

        private Guid FindRelevantKey(Inventory inventory, FillSearcher aSearcher, FillOptions options,
            IEnumerable<RandomKeyNode> availableLocations, IEnumerable<NodeBase> unRetracableKeys, 
            IEnumerable<Guid> restrictedItems, IEnumerable<Guid> prioritizedItems,
            ItemPool pool, Dictionary<string, Guid> itemMap,
            Random random, LogLayer log)
		{
            var relevantKeyLog = log.AddChild("Relevant Key");

            var relevantKeys = new Dictionary<Guid, List<NodeBase>>();
			foreach (var item in pool.AvailableItems().Distinct().Where(key => !restrictedItems.Contains(key)))
			{
				var testInventory = inventory.Expand(KeyManager.GetKey(item));

				var tempSearcher = new FillSearcher(aSearcher);

                var currentKeyLog = relevantKeyLog.AddChild(KeyManager.GetKeyName(item));

                var reachableKeys = new List<NodeBase>();
                bool searchAgain;
                do
                {
                    // Find all nodes that can be reached with current inventory
                    reachableKeys.RemoveAll(node => testInventory.myNodes.Contains(node));
                    reachableKeys.AddRange(tempSearcher.ContinueSearch(testInventory, node => (node is KeyNode) && !testInventory.myNodes.Contains(node)));

                    var combinedReachableKeys = unRetracableKeys.Where(key => !testInventory.myNodes.Contains(key)).Union(reachableKeys);
                    
                    searchAgain = false;

                    var retracableKeys = combinedReachableKeys.AsParallel().Where(node => node == endNode || NodeTraverser.PathExists(node, startNode, node is EventKeyNode ? testInventory.Expand(node) : testInventory)).ToList();

                    if (retracableKeys.Any())
                    {
                        // If any events can be reached, add to inventory and update search before continuing
                        var retracableEvents = retracableKeys.Where(node => node is EventKeyNode).ToList();
                        if (retracableEvents.Any())
                        {
                            currentKeyLog.AddChild("Retracable events", retracableEvents.Select(node => node.Name()));
                            testInventory.myNodes.AddRange(retracableEvents);

                            // Prioritize item that will let you beat the game
                            if (retracableEvents.Any(node => node == endNode))
                            {
                                relevantKeyLog.Message += $" : {KeyManager.GetKeyName(item)}";

                                return item;
                            }

                            searchAgain = true;

                            continue;
                        }

                        var randomizedLocations = retracableKeys.Where(key => key is RandomKeyNode randomNode).Select(key => key as RandomKeyNode).OrderBy(x => x.id).ToList();

                        // Pick up any items already filled in on the map and update search before placing any items
                        var preFilledLocations = randomizedLocations.Where(loc => itemMap.ContainsKey(loc.myRandomKeyIdentifier));

                        if (preFilledLocations.Any())
                        {
                            currentKeyLog.AddChild("Prefilled locations", preFilledLocations.Select(node => $"{node.Name()} - {node.GetKeyName()}"));
                            testInventory.myNodes.AddRange(preFilledLocations);

                            searchAgain = true;

                            continue;
                        }

                        currentKeyLog.AddChild($"Locations", retracableKeys.Select(key => key.Name()));

                        relevantKeys.Add(item, retracableKeys);
                    }
                } while (searchAgain);
			}
            
            // Filter out any keys that by rules cannot be placed in any available location
            var filteredKeys = relevantKeys.Where(key => availableLocations.Any(location => KeyAllowedInLocation(key.Key, options, itemMap, pool, location))).ToDictionary(key => key.Key, key => key.Value);

            relevantKeyLog.AddChild("Filtered Keys", filteredKeys.Select(key => $"{KeyManager.GetKeyName(key.Key)}"));

            if (!filteredKeys.Any())
            {
                return Guid.Empty;
            }

            // Correlate prioritized and relevant keys
            var prioritizedRelevantKeys = filteredKeys.Where(key => prioritizedItems.Contains(key.Key));

            if (prioritizedRelevantKeys.Any())
            {
                filteredKeys = prioritizedRelevantKeys.ToDictionary(key => key.Key, key => key.Value);
            }

            relevantKeyLog.AddChild("Prioritized Relevant Keys", filteredKeys.Select(key => $"{KeyManager.GetKeyName(key.Key)}"));

            // Prioritize items that open up major location for local swaps
            if (options.majorSwap == FillOptions.ItemSwap.LocalPool || options.minorSwap == FillOptions.ItemSwap.LocalPool)
            {
                var keysWithMajorLocation = filteredKeys.Where(key => key.Value.Any(location => StaticKeys.IsMajorLocation(location)));
                if (keysWithMajorLocation.Any())
                {
                    filteredKeys = keysWithMajorLocation.ToDictionary(key => key.Key, key => key.Value);
                }
            }

            // Avoid sprawl calculation bias if value is not set or there is only one item to choose from
            if (options.SprawlFactor == 0 || filteredKeys.Count < 2)
            {
                var item = pool.PeekAmong(filteredKeys.Keys, random);

                relevantKeyLog.Message += $" : {KeyManager.GetKeyName(item)}";
                relevantKeyLog.AddChild($"Item pulled from pool: {KeyManager.GetKeyName(item)}");

                return item;
            }

            relevantKeyLog.AddChild($"Sprawl factor: {options.SprawlFactor}");

            // Translate sprawl factor to a decimal
            var sprawl = ((double)options.SprawlFactor)/10;

            // Weight is numberOfLocationsUnlocked ^ sprawl
            var keysGroupedByWeight = filteredKeys.ToLookup(key => key.Value.Count, key => key.Key).Select(group => new KeyValuePair<double, List<Guid>>(Math.Pow(group.Key, sprawl), group.ToList())).OrderBy(pair => pair.Key);

            relevantKeyLog.AddChild("Keys with weights", keysGroupedByWeight.SelectMany(group => group.Value.Select(key => $"{KeyManager.GetKeyName(key)} - {group.Key}")));

            // Sum of all weights
            var weightSum = keysGroupedByWeight.Select(pair => pair.Key).Sum();

            relevantKeyLog.AddChild($"Sum of Weights: {weightSum}");

            // Pick random number between 0 and weightSum
            var weightRandom = random.NextDouble()* weightSum;

            relevantKeyLog.AddChild($"Weight Random: {weightRandom}");

            // Remove weight from random value until it goes below zero then pick that key
            foreach (var key in keysGroupedByWeight)
            {
                weightRandom -= key.Key;
                if (weightRandom < 0)
                {
                    relevantKeyLog.AddChild($"Chosen weight: {key.Key}");

                    var item = pool.PeekAmong(key.Value, random);

                    relevantKeyLog.Message += $" : {KeyManager.GetKeyName(item)}";
                    relevantKeyLog.AddChild($"Selected Key: {KeyManager.GetKeyName(item)}");

                    return item;
                }
            }

            relevantKeyLog.AddChild($"No Key Selected");
            return Guid.Empty;
		}
    }
}
