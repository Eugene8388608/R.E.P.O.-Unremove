using HarmonyLib;
using UnityEngine;

namespace Unremove;

[HarmonyPatch(typeof(EnemyDirector), nameof(EnemyDirector.Start))]
static class EnemyDirectorPatch
{
    static void Postfix(EnemyDirector __instance)
    {
        if (!Unremove.Instance.configAddEnemyVirus.Value || SemiFunc.IsMultiplayer())
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

    static EnemySetup virus = null!;
}
