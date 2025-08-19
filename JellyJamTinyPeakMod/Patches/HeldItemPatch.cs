using HarmonyLib;
using UnityEngine;
using Photon.Pun;

[HarmonyPatch(typeof(CharacterItems), nameof(CharacterItems.EquipSlotRpc))]
public static class EquipSlotRpcPatch
{
    static void Postfix(CharacterItems __instance, int slotID, int objectViewID)
    {
        if (objectViewID != -1)
        {
            PhotonView photonView = PhotonNetwork.GetPhotonView(objectViewID);
            if (photonView != null)
            {
                Item item = photonView.GetComponent<Item>();
                if (item != null)
                {
                    item.transform.localScale *= 0.6f; 

                    Debug.Log($"[Patch] Scaled item {item.name} to {item.transform.localScale}");
                }
            }
        }
    }
}
