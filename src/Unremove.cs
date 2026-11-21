using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace Unremove;

[BepInPlugin("Eugene8388608.Unremove", "Unremove", "0.6.0")]
public class Unremove : BaseUnityPlugin
{
    internal static Unremove Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger => Instance._logger;
    private ManualLogSource _logger => base.Logger;
    internal Harmony? Harmony { get; set; }
    internal ConfigEntry<bool> configForceAdd = null!;
    internal ConfigEntry<bool> configAddThrowUpgrade = null!;
    internal ConfigEntry<bool> configAddHideyBox = null!;
    internal ConfigEntry<bool> configAddBigCart = null!;
    internal ConfigEntry<bool> configAddManorTVStartRoom = null!;
    internal ConfigEntry<bool> configAddEnemyVirus = null!;
    internal ConfigEntry<bool> configTryFixDebugRefs = null!;
    internal ConfigEntry<bool> configLogPrefabs = null!;

    private void Awake()
    {
        Instance = this;

        // Prevent the plugin from being deleted
        this.gameObject.transform.parent = null;
        this.gameObject.hideFlags = HideFlags.HideAndDontSave;

        const string restartNotice =
            "\n!!! Game restart is required after changing this option !!!";

        configForceAdd = Config.Bind(
            "General", "AddTechnicallyDisabledItems",
            true,
            "As of R.E.P.O v0.5.2 just adds single Walkie Talkies in the shop.\n" +
            "This mod pairs each of them to itself after first use,\n" +
            "which looks odd actually." +

            restartNotice
        );

        configAddThrowUpgrade = Config.Bind(
            "General", "ThrowUpgradeBox",
            false,
            "Whether or not to add the Throw Upgrade shop item.\n" +

            "Please note that players without this mod will see the Upgrade box,\n" +
            "but they won't be able to grab or use it. They also won't see it being\n" +
            "carried away by you, i.e. they won't see the new position of the box." +

            restartNotice
        );

        configAddHideyBox = Config.Bind(
            "General", "AddHideyBox",
            false,
            "Whether or not to add the Hidey Box shop item.\n" +

            "This is literally a blocky box with a cutout for eyes." +

            restartNotice
        );

        configAddBigCart = Config.Bind(
            "General", "AddBigCart",
            false,
            "Whether or not to add the Big C.A.R.T." +

            restartNotice
        );

        configAddManorTVStartRoom = Config.Bind(
            "General", "AddManorTVStartRoom",
            false,
            "Whether or not to add the Start Room - Manor - TV Room.\n" +
            "Disabled by default because it doesn't have an extraction point." +

            restartNotice
        );

        configAddEnemyVirus = Config.Bind(
            "General", "AddEnemyVirus",
            false,
            "Weird immortal yet harmless copy of Mentalist who doesn't\n" +
            "attack. It does spawn scary popups around you.\n" +

            "Restart is not required if changed through REPOConfig mod.\n" +
            "Singleplayer only because its main attraction (The Popups)\n" +
            "cannot be instantiated over Photon."
        );

        configTryFixDebugRefs = Config.Bind(
            "General", "FixVanillaDebugModules",
            true,
            "Debug Command Handler incorrectly registers vanilla debug modules.\n" +
            "An easy fix but you can disable it in case of problems." +

            restartNotice
        );

        configLogPrefabs = Config.Bind(
            "General", "LogAddedPrefabRefs",
            false,
            "Log objects that are being unremoved."
        );

        Patch();

        Logger.LogInfo($"{Info.Metadata.GUID} v{Info.Metadata.Version} has loaded!");
    }

    internal void Patch()
    {
        Harmony ??= new Harmony(Info.Metadata.GUID);
        Harmony.PatchAll();
    }

    internal void Unpatch()
    {
        Harmony?.UnpatchSelf();
    }

    internal static PrefabRef Ref(string resourcePath, bool log = true)
    {
        string prefabName = resourcePath.Split('/')[^1];

        if (log && Instance.configLogPrefabs.Value)
            Logger.LogInfo(prefabName);

        return new()
        {
            resourcePath = resourcePath,
            prefabName = prefabName
        };
    }

    internal static IEnumerable<PrefabRef> Refs(params string[] resourcePaths)
        => resourcePaths.Select(x => Ref(x));
}
