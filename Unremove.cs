using System.Linq;
using BepInEx;
using BepInEx.Logging;
// using HarmonyLib;
using UnityEngine;

namespace Unremove;

[BepInPlugin("Eugene8388608.Unremove", "Unremove", "0.0.1")]
public class Unremove : BaseUnityPlugin
{
    internal static Unremove Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger => Instance._logger;
    private ManualLogSource _logger => base.Logger;
    // internal Harmony? Harmony { get; set; }

    private void Awake()
    {
        Instance = this;
        
        // Prevent the plugin from being deleted
        this.gameObject.transform.parent = null;
        this.gameObject.hideFlags = HideFlags.HideAndDontSave;

        // Patch();

        Logger.LogInfo($"{Info.Metadata.GUID} v{Info.Metadata.Version} has loaded!");
    }

    // ts is broken af
    private Item CreateThrowUpgrade()
    {
        var item = ScriptableObject.CreateInstance<Item>();
        var value = ScriptableObject.CreateInstance<Value>();
        value.valueMin = 250;
        value.valueMax = 500;

        item.disabled            = true;
        item.name = "Item upgrade throwussy";
        item.itemName            = "throwussy";
        // item.itemNameLocalized   = LocalizedAsset;
        // item.description         = string;
        item.itemType            = SemiFunc.itemType.item_upgrade;
        // item.emojiIcon           = SemiFunc.emojiIcon;
        item.itemVolume          = SemiFunc.itemVolume.upgrade;
        // item.itemSecretShopType  = SemiFunc.itemSecretShopType;
        // item.colorPreset         = ColorPresets;
        item.prefab              = new PrefabRef{resourcePath = "Items/Item Upgrade Player Grab Throw"};
        item.value               = value;
        item.maxAmount           = 10;
        item.maxAmountInShop     = 10;
        // item.minPlayerCount      = int;
        // item.maxPurchase         = bool;
        // item.maxPurchaseAmount   = int;
        // item.spawnRotationOffset = Quaternion;
        // item.physicalItem        = bool;

        // TODO: add ItemAttributes
        // item.AddComponent

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
        nuint count = 0;

        var items = Resources.LoadAll<Item>(
            "Items" /* or "Items/Removed Items" */
        )
        .Where(item => item.disabled)
        .Append(CreateThrowUpgrade())
        ;

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
