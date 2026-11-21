using HarmonyLib;
using UnityEngine;

namespace Unremove;

[HarmonyWrapSafe]
[HarmonyPatch(typeof(EnemyDirector), nameof(EnemyDirector.Start))]
static class AddEnemyVirusPatch
{
    static void Postfix(EnemyDirector __instance)
    {
        if (SemiFunc.IsMultiplayer() || !Unremove.Instance.configAddEnemyVirus.Value)
            return;

        __instance.enemiesDifficulty2.Add(virus.Instance);
    }

    internal static readonly ScriptableObjectSingleton<EnemySetup> virus = new(SetupScriptableObject);

    static void SetupScriptableObject(EnemySetup virus)
    {
        var prefabref = Unremove.Ref("Enemies/Enemy - Virus");
        virus.name = prefabref.prefabName;
        virus.spawnObjects = [prefabref];

        var rp = virus.rarityPreset = ScriptableObject.CreateInstance<RarityPreset>();
        rp.chance = 100;
    }
}
