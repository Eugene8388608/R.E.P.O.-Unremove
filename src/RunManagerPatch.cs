using System.Collections.Generic;
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

        // these should be run just once
        PatchArcticStuff(__instance);
        PatchManorStuff(__instance);
        PatchMuseumStuff(__instance);
    }

    static void PatchArcticStuff(RunManager rm)
    {
        var arctic = Find(rm.levels, "Level - Arctic");

        arctic?.ModulesMega1.Add(Unremove.Ref(
            "Level/Arctic/Modules/Module - Arctic - M - 1 - Cargo Central"
        ));

        arctic?.ModulesNormal3.Add(Unremove.Ref(
            "Level/Arctic/Modules/Module - Arctic - N - 3 - Sector S"
        ));

        arctic?.ModulesPassage2.Add(Unremove.Ref(
            "Level/Arctic/Modules/Module - Arctic - P - 2 - Frozen Medbay"
        ));
    }

    static void PatchManorStuff(RunManager rm)
    {
        var manor = Find(rm.levels, "Level - Manor");
        if (!manor) return;

        if (Unremove.Instance.configAddManorTVStartRoom.Value)
            manor.StartRooms.Add(Unremove.Ref(
                "Level/Manor/Start Room/Start Room - Manor - TV Room"
            ));

        Find(manor.ValuablePresets, "Valuables - Manor")
        ?.medium.Add(Unremove.Ref(
            "Valuables/03 Medium/Valuable Manor Marble Table"
        ));
    }

    static void PatchMuseumStuff(RunManager rm)
    {
        Find(rm.levels, "Level - Museum")
        ?.ModulesNormal2.AddRange(Unremove.Refs(
            "Level/Museum/Modules/Module - Museum - N - 2 - Basement Gallery",
            "Level/Museum/Modules/Module - Museum - N - 2 - Smaller rooms and stairs wip"
        ));
    }

    static T? Find<T>(List<T> list, string name)
    where T : UnityEngine.Object
    {
        T obj = list.Find(x => x.name == name);

        if (!obj) Unremove.Logger.LogError($"{name} not found");

        return obj;
    }
}
