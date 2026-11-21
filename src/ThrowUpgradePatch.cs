using HarmonyLib;
using UnityEngine;

namespace Unremove;

// This patch is the reason this mod is NOT host-only

[HarmonyWrapSafe]
[HarmonyPatch(typeof(ItemAttributes), nameof(ItemAttributes.Awake))]
static class ThrowUpgradePatch
{
    static void Prefix(ItemAttributes __instance)
    {
        if (__instance.item || !__instance.GetComponent<ItemUpgradePlayerGrabThrow>())
            return;

        __instance.item = item.Instance;
        __instance.enabled = true;
        __instance.GetComponent<PhysGrabObject>().enabled = true;
        __instance.GetComponent<ItemUpgrade>().enabled = true;
        __instance.GetComponent<Photon.Pun.PhotonTransformView>().enabled = true;
        __instance.GetComponent<NotValuableObject>().enabled = true;
        __instance.GetComponent<RoomVolumeCheck>().enabled = true;
        __instance.GetComponent<PhysGrabObjectImpactDetector>().enabled = true;
        __instance.GetComponent<LineBetweenTwoPoints>().enabled = true;
        __instance.GetComponent<MapCustom>().enabled = true;

        Unremove.Logger.LogInfo("Patched Throw Upgrade");
    }

    internal static readonly ScriptableObjectSingleton<Item> item = new(SetupScriptableObject);

    static void SetupScriptableObject(Item item)
    {
        item.prefab          = Unremove.Ref("Items/Item Upgrade Player Grab Throw");
        item.name            = item.prefab.prefabName;
        item.itemName        = "Throw Upgrade";
        item.itemType        = SemiFunc.itemType.item_upgrade;
        item.itemVolume      = SemiFunc.itemVolume.upgrade;
        item.maxAmount       = 10;
        item.maxAmountInShop = 3;
        // Consider extracting Value preset from the game's assets
        var value = item.value = ScriptableObject.CreateInstance<Value>();
        value.valueMin = 250;
        value.valueMax = 500;
    }
}
