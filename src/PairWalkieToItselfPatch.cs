using HarmonyLib;
using Photon.Pun;
using UnityEngine;

namespace Unremove;

[HarmonyWrapSafe]
[HarmonyPatch(typeof(ItemWalkieTalkie), nameof(ItemWalkieTalkie.Start))]
static class PairWalkieToItselfPatch
{
    static readonly PrefabRef walkieBox = Unremove.Ref(
        "Items/Item WalkieTalkieBox",
        log: false
    );

    // GetMaterialIndex() sets it just in time
    static Material[] walkieMaterials = null!;

    static void Postfix(ItemWalkieTalkie __instance)
    {
        if (
            !SemiFunc.IsMasterClientOrSingleplayer()
            || __instance.otherWalkie is not null
        )
            return;

        int matIndex = GetMaterialIndex(__instance);

        if (!SemiFunc.IsMultiplayer())
        {
            __instance.otherWalkie = __instance;
            __instance.SetMaterial(walkieMaterials[matIndex]);
            Unremove.Logger.LogInfo(
                $"Paired Walkie Talkie to itself, material index {matIndex}"
            );
            return;
        }

        int viewID = __instance.photonView.ViewID;
        // idk man we have walkies box that spawn a couple of singles,
        // and now we have a single one spawning a box
        // rpc d.snuts :P
        var rpcWorkaround = PhotonNetwork.InstantiateRoomObject(
            walkieBox.resourcePath,
            AssetManager.instance.physDisabledPosition,
            Quaternion.identity
        );
        rpcWorkaround.GetPhotonView().RPC(
            "LinkWalkiesRPC", RpcTarget.All,
            viewID, viewID, matIndex
        );
        PhotonNetwork.Destroy(rpcWorkaround);
        Unremove.Logger.LogInfo(
            $"Paired Walkie Talkie to itself (RPC), material index {matIndex}"
        );
    }

    static int GetMaterialIndex(ItemWalkieTalkie walkie)
	{
        var itemAttributes = walkie.GetComponent<ItemAttributes>();

		int index = 0;
		foreach (var spawnedItem in ItemManager.instance.spawnedItems)
		{
            if (!spawnedItem || spawnedItem.item != itemAttributes.item)
                continue;

            if (spawnedItem == itemAttributes)
                break;

            var spawnedWalkie = spawnedItem.GetComponent<ItemWalkieTalkie>();

            if (spawnedWalkie.otherWalkie == spawnedWalkie)
                index++;
		}

        walkieMaterials ??= walkieBox.Prefab.GetComponent<ItemWalkieBox>().walkieMaterials;

		return Mathf.Clamp(index, 0, walkieMaterials.Length - 1);
	}
}
