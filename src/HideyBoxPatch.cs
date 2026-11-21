using HarmonyLib;
using UnityEngine;

namespace Unremove;

// This patch is the reason this mod is NOT host-only

[HarmonyWrapSafe]
[HarmonyPatch(typeof(ItemAttributes), nameof(ItemAttributes.Awake))]
static class HideyBoxPatch
{
    static void Prefix(ItemAttributes __instance)
    {
        if (
            __instance.item?.itemName != "Leaf Blower"
            || __instance.GetComponent<ItemLeafBlower>()
        )
            return;

        __instance.item = item.Instance;

        Unremove.Logger.LogInfo("Patched Hidey Box");
    }

    internal static readonly ScriptableObjectSingleton<Item> item = new(SetupScriptableObject);

    static void SetupScriptableObject(Item item)
    {
        item.prefab             = Unremove.Ref("Items/Item Hidey Box");
        item.name               = item.prefab.prefabName;
        item.itemName           = "Hidey Box";
        item.itemVolume         = SemiFunc.itemVolume.large;
        item.itemSecretShopType = SemiFunc.itemSecretShopType.shop_attic;
        // Consider extracting Value preset from the game's assets
        var value = item.value = ScriptableObject.CreateInstance<Value>();
        value.valueMin = 250;
        value.valueMax = 500;
    }
}
