using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace Unremove;

// REPOLib is not designed to add builtin items,
// so I might as well just patch whatever I want.
//
// Consider using Harmony Transpiler on StatsManager.LoadItemsFromFolder instead,
// though this sounds more like a task for REPOLib
[HarmonyPatch(typeof(Resources), nameof(Resources.LoadAll), [typeof(string), typeof(System.Type)])]
static class ResourcesLoadAllPatch
{
    static Object[] Postfix(Object[] original, string path, System.Type systemTypeInstance)
    {        
        if (systemTypeInstance != typeof(Item) || path.ToLowerInvariant() != "items/items")
            return original;

        var removedItems = Resources.LoadAll<Item>("Items/Removed Items")
        .Where(
            item =>
            {
                if (item.disabled || Unremove.Instance.configForceAdd.Value)
                {
                    item.disabled = false; // ew side effect
                    return true;
                }
                return false;
            }
        );

        if (Unremove.Instance.configAddThrowUpgrade.Value)
            removedItems = removedItems.Append(ThrowUpgradeItemFactory.Item);

        var removedItemsArray = removedItems.ToArray();

        Unremove.Logger.LogInfo(
            $"{removedItemsArray.Count()} disabled items: " +
            string.Join(", ", removedItemsArray.Select(x => x.itemName))
        );

        return [.. original, .. removedItemsArray];
    }
}
