using HarmonyLib;
using Photon.Pun;

namespace Unremove;

// This patch is the reason this mod is NOT host-only

[HarmonyPatch(typeof(ItemAttributes), nameof(ItemAttributes.Awake))]
static class ThrowUpgradePatch
{
    static void Prefix(ItemAttributes __instance)
    {
        if (!__instance.GetComponent<ItemUpgradePlayerGrabThrow>())
            return;

        __instance.item = ThrowUpgradeItemFactory.Item;
        __instance.enabled = true;
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
