using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
// using HarmonyLib;
using UnityEngine;

namespace Unremove;

[BepInPlugin("Eugene8388608.Unremove", "Unremove", "0.1.0")]
public class Unremove : BaseUnityPlugin
{
    internal static Unremove Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger => Instance._logger;
    private ManualLogSource _logger => base.Logger;
    // internal Harmony? Harmony { get; set; }
    private ConfigEntry<bool> configAddThrowUpgrade = null!;

    private void Awake()
    {
        Instance = this;
        
        // Prevent the plugin from being deleted
        this.gameObject.transform.parent = null;
        this.gameObject.hideFlags = HideFlags.HideAndDontSave;

        // Patch();

        configAddThrowUpgrade = Config.Bind(
            "General", "ThrowUpgradeBox",
            false,
            "Whether or not to add the Throw Upgrade shop item.\n" +

            "Please note that players without this mod will see the Upgrade box,\n" +
            "but they won't be able to grab or use it. They also won't see it being\n" +
            "carried away by you, i.e. they won't see the new position of the box.\n" +

            "!!! Game restart is required after changing this option !!!"
        );

        Logger.LogInfo($"{Info.Metadata.GUID} v{Info.Metadata.Version} has loaded!");
    }

    private Item CreateAndPatchThrowUpgrade()
    {
        var item = ScriptableObject.CreateInstance<Item>();

        item.itemName        = "Throw Upgrade";
        item.itemType        = SemiFunc.itemType.item_upgrade;
        item.itemVolume      = SemiFunc.itemVolume.upgrade;
        // Consider extracting Value preset from the game's assets
        item.value           = ScriptableObject.CreateInstance<Value>();
        item.value.valueMin  = 250;
        item.value.valueMax  = 500;
        item.maxAmount       = 10;
        item.maxAmountInShop = 10;

        item.prefab          = new PrefabRef{
            resourcePath = "Items/Item Upgrade Player Grab Throw"
        };

        var go = item.prefab.Prefab;
        // Consider patching instances of this GameObject
        // instead of relying on the prefab being cached
        go.hideFlags = HideFlags.HideAndDontSave;

        item.name = go.name;

        // The following is the reason this mod is NOT host-only
        var ia = go.GetComponent<ItemAttributes>();
        ia.item = item;
        ia.enabled = true;
        go.GetComponent<PhysGrabObject>().enabled = true;
        go.GetComponent<ItemUpgrade>().enabled = true;
        go.GetComponent<Photon.Pun.PhotonTransformView>().enabled = true;
        go.GetComponent<NotValuableObject>().enabled = true;
        go.GetComponent<RoomVolumeCheck>().enabled = true;
        go.GetComponent<PhysGrabObjectImpactDetector>().enabled = true;
        go.GetComponent<LineBetweenTwoPoints>().enabled = true;
        go.GetComponent<MapCustom>().enabled = true;

        return item;
    }

    // internal void Patch()
    // {
    //     Harmony ??= new Harmony(Info.Metadata.GUID);
    //     Harmony.PatchAll();
    // }

    // internal void Unpatch()
    // {
    //     Harmony?.UnpatchSelf();
    // }

    // TODO: consider patching method(s) instead
    //
    // The game adds items just before loading a playable level
    // This mod adds them just before the splash screen
    private bool itemsUnremoved = false;
    private void Update()
    {
        if (itemsUnremoved || StatsManager.instance is null) return;
    
        itemsUnremoved = true;

        var items = Resources.LoadAll<Item>(
            "Items" /* or "Items/Removed Items" */
        )
        .Where(item => item.disabled);

        var throwUpgrade = CreateAndPatchThrowUpgrade();

        if (configAddThrowUpgrade.Value)
            items = items.Append(throwUpgrade);

        nuint count = 0;
        foreach (var item in items)
        {
            try
            {
                // This is NOT how you add a custom Item to the game
                // Please refer to https://repomods.com/apis/repolib/overview.html
                item.disabled = false;
                StatsManager.instance.itemDictionary[item.name] = item;

                count++;
                Logger.LogInfo(item.name);
            }
            catch (System.Exception e)
            {
                Logger.LogError($"Exception while adding {item?.name}: {e}");
            }
        }
        Logger.LogInfo($"Enabled {count} removed items");

        // TODO: fix icon on magnet orb
        // var magnet = StatsManager.instance.itemDictionary["Item Orb Magnet"];
        // magnet.GameObject
    }
}
