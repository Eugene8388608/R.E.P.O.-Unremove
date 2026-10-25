using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace Unremove;

[BepInPlugin("Eugene8388608.Unremove", "Unremove", "0.3.0")]
public class Unremove : BaseUnityPlugin
{
    internal static Unremove Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger => Instance._logger;
    private ManualLogSource _logger => base.Logger;
    internal Harmony? Harmony { get; set; }
    internal ConfigEntry<bool> configAddThrowUpgrade = null!;
    internal ConfigEntry<bool> configForceAdd = null!;
    internal ConfigEntry<bool> configAddManorTVStartRoom = null!;

    private void Awake()
    {
        Instance = this;

        // Prevent the plugin from being deleted
        this.gameObject.transform.parent = null;
        this.gameObject.hideFlags = HideFlags.HideAndDontSave;

        configAddThrowUpgrade = Config.Bind(
            "General", "ThrowUpgradeBox",
            false,
            "Whether or not to add the Throw Upgrade shop item.\n" +

            "Please note that players without this mod will see the Upgrade box,\n" +
            "but they won't be able to grab or use it. They also won't see it being\n" +
            "carried away by you, i.e. they won't see the new position of the box.\n" +

            "!!! Game restart is required after changing this option !!!"
        );

        configForceAdd = Config.Bind(
            "General", "AddTechnicallyDisabledItems",
            true,
            "As of R.E.P.O v0.4.4 just adds single Walkie Talkies in the shop.\n" +
            "This mod pairs each of them to itself after first use,\n" +
            "which looks odd actually.\n" +

            "!!! Game restart is required after changing this option !!!"
        );

        configAddManorTVStartRoom = Config.Bind(
            "General", "AddManorTVStartRoom",
            false,
            "Whether or not to add the Start Room - Manor - TV Room.\n" +
            "Disabled by default because it doesn't have an extraction point.\n" +

            "!!! Game restart is required after changing this option !!!"
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
        if (log) Logger.LogInfo(prefabName);
        return new()
        {
            resourcePath = resourcePath,
            prefabName = prefabName
        };
    }
}
