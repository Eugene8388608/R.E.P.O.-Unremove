using HarmonyLib;
using Photon.Pun;
using UnityEngine;

namespace Unremove;

[HarmonyPatch(typeof(ItemWalkieTalkie), nameof(ItemWalkieTalkie.Start))]
static class PairTalkieToItselfPatch
{
    static void Postfix(ItemWalkieTalkie __instance)
    {
        if (__instance.otherWalkie is not null) return;

        if (SemiFunc.IsMasterClient())
        {
            int viewID = __instance.photonView.ViewID;
            // idk man we have walkies box that spawn a couple of singles,
            // and now we have a single one spawning a box
            // rpc d.snuts :P
            var rpcWorkaround = PhotonNetwork.InstantiateRoomObject(
                "Items/Item WalkieTalkieBox",
                Vector3.zero,
                Quaternion.identity,
                0, null
            );
            rpcWorkaround.GetPhotonView().RPC("LinkWalkiesRPC", RpcTarget.All, [viewID, viewID, 0]);
            PhotonNetwork.Destroy(rpcWorkaround);
            Unremove.Logger.LogInfo("Paired Walkie Talkie to itself (RPC)");
        }
        else if (!SemiFunc.IsMultiplayer())
        {
            __instance.otherWalkie = __instance;
            Unremove.Logger.LogInfo("Paired Walkie Talkie to itself");
        }
    }
}
