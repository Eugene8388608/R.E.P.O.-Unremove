using HarmonyLib;
using Photon.Pun;

namespace Unremove;

[HarmonyPatch(typeof(ItemUpgradePlayerGrabThrow), nameof(ItemUpgradePlayerGrabThrow.Start))]
static class ItemUpgradePlayerGrabThrowPatch
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
