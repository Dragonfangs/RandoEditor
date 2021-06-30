
using Common.Key;
using Common.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Verifier.ItemRules
{
    public class ItemRuleUtility
    {
        public static Dictionary<Guid, List<String>> GetRequiredLocationRules(List<ItemRuleBase> rules)
        {
            var itemRequiredLocationRules = rules.Where(rest => rest is ItemRuleInLocation).Select(rest => rest as ItemRuleInLocation).ToLookup(rest => rest.ItemId, rest => rest.LocationIdentifier).ToDictionary(group => group.Key, group => group.ToList());
            var itemBlockedLocationRules = rules.Where(rest => rest is ItemRuleNotInLocation).Select(rest => rest as ItemRuleNotInLocation).ToLookup(rest => rest.ItemId, rest => rest.LocationIdentifier).ToDictionary(group => group.Key, group => group.ToList()); ;
            
            // Remove redundant rules
            foreach (var rule in itemBlockedLocationRules)
            {
                if (itemRequiredLocationRules.ContainsKey(rule.Key))
                {
                    var overlappingLocations = rule.Value.Where(loc => itemRequiredLocationRules[rule.Key].Contains(loc)).ToList();

                    if (overlappingLocations.Any())
                    {
                        itemRequiredLocationRules[rule.Key].RemoveAll(loc => overlappingLocations.Contains(loc));
                    }

                    if (!itemRequiredLocationRules[rule.Key].Any())
                    {
                        itemRequiredLocationRules.Remove(rule.Key);
                    }
                }
            }

            return itemRequiredLocationRules;
        }

        public static Dictionary<Guid, List<String>> GetBlockedLocationRules(List<ItemRuleBase> rules)
        {
            return rules.Where(rest => rest is ItemRuleNotInLocation).Select(rest => rest as ItemRuleNotInLocation).ToLookup(rest => rest.ItemId, rest => rest.LocationIdentifier).ToDictionary(group => group.Key, group => group.ToList()); ;
        }

        public static bool VerifyLocationRules(List<ItemRuleBase> rules, Dictionary<string, Guid> itemMap, LogLayer detailedLog)
        {
            var verifyLog = detailedLog.AddChild("Verify Location Rules");

            var itemRequiredLocationRules = GetRequiredLocationRules(rules);

            var requiredLog = verifyLog.AddChild("RequiredLocationRules");
            
            foreach (var rule in itemRequiredLocationRules)
            {
                var itemLog = requiredLog.AddChild(KeyManager.GetKeyName(rule.Key));
                itemLog.AddChild("Required Location(s)", rule.Value);

                var actualLocations = itemMap.Where(item => item.Value == rule.Key);
                itemLog.AddChild("Actual Location(s)", actualLocations.Select(loc => loc.Key));

                if (!actualLocations.Any())
                {
                    itemLog.AddChild("Item not placed at all, skipping");
                    continue;
                }

                var overlappingLocations = actualLocations.Where(loc => rule.Value.Contains(loc.Key));
                itemLog.AddChild("Overlapping Location(s)", overlappingLocations.Select(loc => loc.Key));
                if (!overlappingLocations.Any())
                {
                    itemLog.AddChild("No overlapping locations");
                    return false;
                }
            }

            verifyLog.AddChild("All Required Location rules verified");

            var itemBlockedLocationRules = GetBlockedLocationRules(rules);

            var blockedLog = verifyLog.AddChild("BlockedLocationRules");

            foreach (var rule in itemBlockedLocationRules)
            {
                var itemLog = blockedLog.AddChild(KeyManager.GetKeyName(rule.Key));
                itemLog.AddChild("Blocked Location(s)", rule.Value);

                var actualLocations = itemMap.Where(item => item.Value == rule.Key);
                itemLog.AddChild("Actual Location(s)", actualLocations.Select(loc => loc.Key));

                if (!actualLocations.Any())
                {
                    itemLog.AddChild("Item not placed at all, skipping");
                    continue;
                }

                var incorrectLocations = actualLocations.Where(loc => rule.Value.Contains(loc.Key));
                if (incorrectLocations.Any())
                {
                    itemLog.AddChild("Incorrect Location(s)", incorrectLocations.Select(loc => loc.Key));
                    return false;
                }
            }

            verifyLog.AddChild("All Blocked Location rules verified");

            return true;
        }
    }
}
