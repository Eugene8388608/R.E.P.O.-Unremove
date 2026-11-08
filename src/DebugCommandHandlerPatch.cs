using HarmonyLib;

namespace Unremove;

[HarmonyPatch(typeof(DebugCommandHandler), nameof(DebugCommandHandler.RegisterDevCommands))]
static class DebugCommandHandlerPatch
{
    static void Postfix(DebugCommandHandler __instance)
    {
        if (Unremove.Instance.configTryFixDebugRefs.Value)
            foreach (var p in __instance.debugModules)
                p.resourcePath ??= "Level/Debug/Modules/" + p.prefabName;

        __instance.debugModules.AddRange(Unremove.Refs(
            // === pre v0.4.0 shop ===
            "Level/Shop/Modules/Old/Module - Shop - DE - Bathroom 1",
            "Level/Shop/Modules/Old/Module - Shop - DE - Big Bathroom",
            "Level/Shop/Modules/Old/Module - Shop - DE - Storage",

            // === unfinished ===

            // early copy of M - 2 - Mama Spewer
            "Level/Arctic/Modules/Module - Arctic - M - 1 - Container Yard",

            // a scooter and a bit of valuables
            "Level/Manor/Modules/Module - Manor - M - 2 - Silly Cemetery",

            // early copy of M - 2 - Whale World
            "Level/Manor/Modules/Module - Manor - M - 2 - Whale room 1",

            // no valuables, holes in walls
            "Level/Wizard/Modules/Module - Wizard - M - 1 - Crypts", 

            // === unplayable ===

            // ground missing
            "Level/Desert/Modules/Module - Desert - DE - 3 - Minecart to hole",

            // no doors, "we'll miss you, susan!"
            "Level/Desert/Modules/Module - Desert - N - 1 - Employee Break Room",

            // didn't manage to break the door
            "Level/Shop/Modules/Module - Shop - DE - Barricaded Door",

            // === debug ===
            "Level/Manor/Modules/Module - M - Template",
            "Level/Manor/Modules/Module - Manor - DE - 1 - Template",
            "Level/Manor/Modules/Module - Manor - M - 1 - Earth Basement ruben wip",
            "Level/Manor/Modules/Module - N - Template",
            "Level/Museum/Modules/Module - Museum - M - ruben laser pontus",
            "Level/Shop/Modules/Module - Shop - DE - Secret Template"

            // walls are missing sometimes, obstructed valuables, missing props, too dark
            // "Level/Manor/Modules/Module - Manor - N - 2 - Dining Hall 2"
            // R.E.P.O. v0.5.0 update: released as N - 2 - Gallery Hall

            // no valuables
            // "Level/Museum/Modules/Module - Museum - N - 1 - Monika's Room"
            // R.E.P.O. v0.5.0 update: gone
        ));
    }
}
