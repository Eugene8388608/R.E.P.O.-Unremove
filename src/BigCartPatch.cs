using HarmonyLib;
using UnityEngine;

namespace Unremove;

// This patch is the reason this mod is NOT host-only

[HarmonyWrapSafe]
[HarmonyPatch(typeof(ItemAttributes), nameof(ItemAttributes.Awake))]
static class BigCartPatch
{
    static void Prefix(ItemAttributes __instance)
    {
        if (__instance.item || !__instance.GetComponent<PhysGrabCart>())
            return;

        __instance.item = item.Instance;

        // parity with other carts
        __instance.gameObject.AddComponent<SpectateCameraCollisionIgnore>();

        // destroy copypaste leftover medium cart mesh
        foreach (Transform t in __instance.transform)
            if (t.name == "Cart Mesh" && t.childCount == 2)
            {
                Object.Destroy(t.gameObject);
                break;
            }

        // move the value display somewhere where it'll be visible
        var screen = __instance.transform.Find("Screen");
        screen.localPosition = new(0, -0.04f, -1.145f);
        screen.localRotation = screen.Find("Text").localRotation;

        // that blue backgrund
        Object.Destroy(screen.Find("Cube (13)").gameObject);

        // repurpose value display holder for the handle
        var panel = screen.Find("Display Wide");
        panel.localScale = new(0.8f, 0.8f, 0.8f);
        panel.localPosition = new(0, 0.26f, -0.5f);
        panel.localRotation = Quaternion.Euler(69, 0, 180);

        Unremove.Logger.LogInfo("Patched Big C.A.R.T.");
    }

    internal static readonly ScriptableObjectSingleton<Item> item = new(SetupScriptableObject);

    static void SetupScriptableObject(Item item)
    {
        item.prefab          = Unremove.Ref("Items/Cart Big");
        item.name            = item.prefab.prefabName;
        item.itemName        = "C.A.R.T.E.S.I.A.N.";
        item.itemVolume      = SemiFunc.itemVolume.large_wide;
        item.itemType        = SemiFunc.itemType.cart;
        item.maxAmount       = 2;
        // Consider extracting Value preset from the game's assets
        var value = item.value = ScriptableObject.CreateInstance<Value>();
        value.valueMin = 9500;
        value.valueMax = 12000;
    }
}
