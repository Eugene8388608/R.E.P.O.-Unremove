using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Photon.Pun;
using UnityEngine;

namespace Unremove;

[BepInPlugin("Eugene8388608.Unremove", "Unremove", "0.2.0")]
public class Unremove : BaseUnityPlugin
{
    internal static Unremove Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger => Instance._logger;
    private ManualLogSource _logger => base.Logger;
    internal Harmony? Harmony { get; set; }
    internal ConfigEntry<bool> configAddThrowUpgrade = null!;
    internal ConfigEntry<bool> configForceAdd = null!;

    private void Awake()
    {
        Instance = this;
        
        // Prevent the plugin from being deleted
        this.gameObject.transform.parent = null;
        this.gameObject.hideFlags = HideFlags.HideAndDontSave;

        configAddThrowUpgrade = Config.Bind(
            "General", "ThrowUpgradeBox",
            false,
            "Whether or not to add the Throw Upgrade shop item.\n" +

            "Please note that players without this mod will see the Upgrade box,\n" +
            "but they won't be able to grab or use it. They also won't see it being\n" +
            "carried away by you, i.e. they won't see the new position of the box.\n" +

            "!!! Game restart is required after changing this option !!!"
        );

        configForceAdd = Config.Bind(
            "General", "AddTechnicallyDisabledItems",
            true,
            "As of R.E.P.O v0.4.4 just adds single Walkie Talkies in the shop.\n" +
            "This mod pairs each of them to itself after first use,\n" +
            "which looks odd actually.\n" +

            "!!! Game restart is required after changing this option !!!"
        );

        Patch();

        Logger.LogInfo($"{Info.Metadata.GUID} v{Info.Metadata.Version} has loaded!");
    }

    internal void Patch()
    {
        Harmony ??= new Harmony(Info.Metadata.GUID);
        Harmony.PatchAll();
    }

    internal void Unpatch()
    {
        Harmony?.UnpatchSelf();
    }
}

class ThrowUpgradeItemFactory
{
    const string throwUpgradeLongName = "Item Upgrade Player Grab Throw";
    const string throwUpgradePath = "Items/" + throwUpgradeLongName;

    private static Item _item = null!;
    internal static Item Item
    {
        get
        {
            if (!_item) Instantiate();
            return _item;
        }
    }

    private static void Instantiate()
    {
        _item = ScriptableObject.CreateInstance<Item>();

        _item.name            = throwUpgradeLongName;
        _item.itemName        = "Throw Upgrade";
        _item.itemType        = SemiFunc.itemType.item_upgrade;
        _item.itemVolume      = SemiFunc.itemVolume.upgrade;
        // Consider extracting Value preset from the game's assets
        _item.value           = ScriptableObject.CreateInstance<Value>();
        _item.value.valueMin  = 250;
        _item.value.valueMax  = 500;
        _item.maxAmount       = 10;
        _item.maxAmountInShop = 10;
        _item.prefab          = new PrefabRef{
            prefabName = throwUpgradeLongName,
            resourcePath = throwUpgradePath
        };
    }
}

// REPOLib is not designed to add builtin items,
// so I might as well just patch whatever I want.
//
// Consider using Harmony Transpiler on StatsManager.LoadItemsFromFolder instead,
// though this sounds more like a task for REPOLib
[HarmonyPatch(typeof(Resources), nameof(Resources.LoadAll), [typeof(string), typeof(System.Type)])]
class ResourcesLoadAllPatch
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

[HarmonyPatch(typeof(ItemUpgradePlayerGrabThrow), nameof(ItemUpgradePlayerGrabThrow.Start))]
class ItemUpgradePlayerGrabThrowPatch
{
    static void Postfix(ItemUpgradePlayerGrabThrow __instance)
    {
        // The following is the reason this mod is NOT host-only
        var ia = __instance.GetComponent<ItemAttributes>();
        ia.item = ThrowUpgradeItemFactory.Item;
        ia.enabled = true;
        __instance.GetComponent<PhysGrabObject>().enabled = true;
        __instance.GetComponent<ItemUpgrade>().enabled = true;
        __instance.GetComponent<PhotonTransformView>().enabled = true;
        __instance.GetComponent<NotValuableObject>().enabled = true;
        __instance.GetComponent<RoomVolumeCheck>().enabled = true;
        __instance.GetComponent<PhysGrabObjectImpactDetector>().enabled = true;
        __instance.GetComponent<LineBetweenTwoPoints>().enabled = true;
        __instance.GetComponent<MapCustom>().enabled = true;
    
        Unremove.Logger.LogInfo("Patched Throw Upgrade");
    }
}

[HarmonyPatch(typeof(ItemWalkieTalkie), nameof(ItemWalkieTalkie.Start))]
class PairToItselfPatch
{
    static void Postfix(ItemWalkieTalkie __instance)
    {
        if (__instance.otherWalkie is not null) return;

        if (SemiFunc.IsMasterClient())
        {
            int viewID = __instance.photonView.ViewID;
            // idk man we have walkies box that spawn a couple of singles,
            // and now we have a single one spawning a box
            // rpc d.snuts :P
			var rpcWorkaround = PhotonNetwork.InstantiateRoomObject(
                "Items/Item WalkieTalkieBox",
                Vector3.zero,
                Quaternion.identity,
                0, null
            );
            rpcWorkaround.GetPhotonView().RPC("LinkWalkiesRPC", RpcTarget.All, [viewID, viewID, 0]);
            PhotonNetwork.Destroy(rpcWorkaround);
            Unremove.Logger.LogInfo("Paired Walkie Talkie to itself (RPC)");
        }
        else if (!SemiFunc.IsMultiplayer())
        {
            __instance.otherWalkie = __instance;
            Unremove.Logger.LogInfo("Paired Walkie Talkie to itself");
        }
    }
}

[HarmonyPatch(typeof(RunManager), nameof(RunManager.Awake))]
class AddMarbleTablePatch
{
    const string levelname = "Level - Manor";
    const string presetname = "Valuables - Manor";
    const string marbleTableLongName = "Valuable Manor Marble Table";
    const string marbleTablePath = "Valuables/03 Medium/" + marbleTableLongName;

    static bool patchFired = false;
    static void Postfix(RunManager __instance)
    {
        if (patchFired) return;

        do
        {
            var level = __instance.levels.Find(x => x.name == levelname);
            if (!level)
            {
                Unremove.Logger.LogWarning($"{levelname} not found");
                break;
            }

            var preset = level.ValuablePresets.Find(x => x.name == presetname);
            if (!preset)
            {
                Unremove.Logger.LogWarning($"{presetname} not found");
                break;
            }

            preset.medium.Add(new PrefabRef{
                resourcePath = marbleTablePath,
                prefabName = marbleTableLongName
            });

            Unremove.Logger.LogInfo(marbleTableLongName);
        } while (false);

        patchFired = true;
    }
}
