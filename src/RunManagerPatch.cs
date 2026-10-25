using HarmonyLib;

namespace Unremove;

[HarmonyPatch(typeof(RunManager), nameof(RunManager.Awake))]
static class RunManagerPatch
{
    static void Postfix(RunManager __instance)
    {
        // bruh idk if this breaks anything
        // it works for me
        // please let me know on discord or github issues
        Unremove.Instance.Harmony?.Unpatch(
            AccessTools.Method(typeof(RunManager), nameof(RunManager.Awake)),
            AccessTools.Method(typeof(RunManagerPatch), nameof(Postfix))
        );

        // these should run just once
        PatchManorStuff(__instance);
        PatchMuseumStuff(__instance);
        AddDebugModules(__instance);
    }

    static void PatchManorStuff(RunManager rm)
    {
        const string levelname = "Level - Manor";
        var level = rm.levels.Find(x => x.name == levelname);
        if (!level)
        {
            Unremove.Logger.LogError($"{levelname} not found");
            return;
        }

        if (Unremove.Instance.configAddManorTVStartRoom.Value)
            level.StartRooms.Add(Unremove.Ref(
                "Level/Manor/Start Room/Start Room - Manor - TV Room"
            ));

        const string presetname = "Valuables - Manor";
        var preset = level.ValuablePresets.Find(x => x.name == presetname);
        if (!preset)
        {
            Unremove.Logger.LogError($"{presetname} not found");
            return;
        }

        preset.medium.Add(Unremove.Ref(
            "Valuables/03 Medium/Valuable Manor Marble Table"
        ));
    }

    static void PatchMuseumStuff(RunManager rm)
    {
        const string levelname = "Level - Museum";
        var level = rm.levels.Find(x => x.name == levelname);
        if (!level)
        {
            Unremove.Logger.LogError($"{levelname} not found");
            return;
        }

        level.ModulesNormal2.AddRange([
            Unremove.Ref("Level/Museum/Modules/Module - Museum - N - 2 - Basement Gallery"),
            Unremove.Ref("Level/Museum/Modules/Module - Museum - N - 2 - Smaller rooms and stairs wip")
        ]);
    }

    static void AddDebugModules(RunManager rm)
    {
        rm.debugModules.AddRange([
            // walls are missing sometimes, obstructed valuables, missing props, too dark
            Unremove.Ref("Level/Manor/Modules/Module - Manor - N - 2 - Dining Hall 2"),

            // no valuables
            Unremove.Ref("Level/Museum/Modules/Module - Museum - N - 1 - Monika's Room")
        ]);
    }
}
