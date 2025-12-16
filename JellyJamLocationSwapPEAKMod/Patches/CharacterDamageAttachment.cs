using System;
using System.Linq;
using UnityEngine;
using BepInEx;
using Photon.Pun;

public class RandomDamageTeleport : MonoBehaviour
{
    private CharacterAfflictions _afflictions;
    private static float lastSwapTime = 0f;
    private static float swapCooldown = 1f; // 1 second cooldown

    private void Awake()
    {
        _afflictions = GetComponent<CharacterAfflictions>();
        if (_afflictions != null)
            _afflictions.OnAddedStatus += OnStatusAdded;
    }

    private void OnStatusAdded(CharacterAfflictions.STATUSTYPE status, float value)
    {
        // Only trigger on damage
        if (status == CharacterAfflictions.STATUSTYPE.Weight || status == CharacterAfflictions.STATUSTYPE.Hunger) return;

        // Respect cooldown
        if (Time.time - lastSwapTime < swapCooldown) return;
        lastSwapTime = Time.time;

        if (!PhotonNetwork.IsMasterClient) return;

        // Find all other active characters
        var others = Character.AllCharacters.Where(c => c != GetComponent<Character>()).ToArray();
        if (others.Length == 0) return;

        // Pick a random target
        var targetCharacter = others[UnityEngine.Random.Range(0, others.Length)];

        // Swap positions
        SwapWith(targetCharacter);
    }

    private void SwapWith(Character target)
    {
        var selfChar = GetComponent<Character>();
        if (selfChar == null || target == null) return;

        Vector3 selfPos = selfChar.Center;
        Vector3 targetPos = target.Center + Vector3.forward * 2f;

        // Teleport both players
        selfChar.photonView.RPC("WarpPlayerRPC", RpcTarget.All, targetPos, false);
        target.photonView.RPC("WarpPlayerRPC", RpcTarget.All, selfPos, false);

        Debug.Log($"[RandomDamageTeleport] Swapped {selfChar.name} with {target.name}");
    }
}
