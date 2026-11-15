using HarmonyLib;
using UnityEngine;

namespace Unremove;

[HarmonyWrapSafe]
[HarmonyPatch(typeof(EnemyDirector), nameof(EnemyDirector.Start))]
static class EnemyDirectorPatch
{
    static EnemySetup virus = null!;

    static void Postfix(EnemyDirector __instance)
    {
        if (SemiFunc.IsMultiplayer() || !Unremove.Instance.configAddEnemyVirus.Value)
            return;

        if (!virus)
        {
            virus = ScriptableObject.CreateInstance<EnemySetup>();

            var prefabref = Unremove.Ref("Enemies/Enemy - Virus");
            virus.name = prefabref.prefabName;
            virus.spawnObjects = [prefabref];

            var rp = ScriptableObject.CreateInstance<RarityPreset>();
            rp.chance = 100;
            virus.rarityPreset = rp;
        }

        __instance.enemiesDifficulty2.Add(virus);
    }
}
