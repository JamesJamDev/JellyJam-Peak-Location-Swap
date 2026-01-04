using System;
using System.Linq;
using UnityEngine;
using BepInEx;
using BepInEx.Configuration;
using Photon.Pun;

public class RandomDamageTeleport : MonoBehaviour
{
    private CharacterAfflictions _afflictions;
    private static float lastSwapTime = 0f;

    private void Awake()
    {
        _afflictions = GetComponent<CharacterAfflictions>();
        if (_afflictions != null)
            _afflictions.OnAddedStatus += OnStatusAdded;
    }

    private void OnStatusAdded(CharacterAfflictions.STATUSTYPE status, float value)
    {
        var selfChar = GetComponent<Character>();
        if (selfChar == null) return;

        if (PhotonNetwork.IsMasterClient)
        {
            TrySwap(selfChar, status, value);
        }
        else
        {
            // Forward to master; ensure this component is on the same GO as the Character's PhotonView
            if (selfChar.photonView != null)
            {
                selfChar.photonView.RPC("RequestSwapFromClient", RpcTarget.MasterClient, selfChar.photonView.ViewID, (int)status, value);
            }
        }
    }

    [PunRPC]
    public void RequestSwapFromClient(int viewId, int statusInt, float amount)
    {
        if (!PhotonNetwork.IsMasterClient) return; // double-guard
        var pv = PhotonView.Find(viewId);
        if (pv == null) return;
        var ch = pv.GetComponent<Character>();
        if (ch == null) return;
        TrySwap(ch, (CharacterAfflictions.STATUSTYPE)statusInt, amount);
    }

    private void TrySwap(Character selfChar, CharacterAfflictions.STATUSTYPE status, float value)
    {
        // Respect global enable and minimum
        if (!JellyJamLocationSwapPEAKMod.SwapDamageConfig.IsEnabled.Value) return;
        Debug.Log("Status added: " + status + " with value: " + value);
        if (value / 1000 < JellyJamLocationSwapPEAKMod.SwapDamageConfig.MinimumAmount.Value) return;

        // Respect per-status toggles
        if (status == CharacterAfflictions.STATUSTYPE.Hunger && !JellyJamLocationSwapPEAKMod.SwapDamageConfig.ToggleHunger.Value) return;
        if (status == CharacterAfflictions.STATUSTYPE.Weight && !JellyJamLocationSwapPEAKMod.SwapDamageConfig.ToggleWeight.Value) return;
        if (status == CharacterAfflictions.STATUSTYPE.Poison && !JellyJamLocationSwapPEAKMod.SwapDamageConfig.TogglePoison.Value) return;
        if (status == CharacterAfflictions.STATUSTYPE.Hot && !JellyJamLocationSwapPEAKMod.SwapDamageConfig.ToggleHot.Value) return;
        if (status == CharacterAfflictions.STATUSTYPE.Cold && !JellyJamLocationSwapPEAKMod.SwapDamageConfig.ToggleCold.Value) return;
        if (status == CharacterAfflictions.STATUSTYPE.Spores && !JellyJamLocationSwapPEAKMod.SwapDamageConfig.ToggleSpores.Value) return;
        if (status == CharacterAfflictions.STATUSTYPE.Injury && !JellyJamLocationSwapPEAKMod.SwapDamageConfig.ToggleInjury.Value) return;
        if (status == CharacterAfflictions.STATUSTYPE.Thorns && !JellyJamLocationSwapPEAKMod.SwapDamageConfig.ToggleThorns.Value) return;
        if (status == CharacterAfflictions.STATUSTYPE.Drowsy && !JellyJamLocationSwapPEAKMod.SwapDamageConfig.ToggleDrowsy.Value) return;
        if (status == CharacterAfflictions.STATUSTYPE.Curse && !JellyJamLocationSwapPEAKMod.SwapDamageConfig.ToggleCurse.Value) return;

        // Cooldown (master-side)
        if (Time.time - lastSwapTime < JellyJamLocationSwapPEAKMod.SwapDamageConfig.SwapCooldown.Value) return;

        var candidates = JellyJamLocationSwapPEAKMod.SwapDamageConfig.AllowCarried.Value
            ? Character.AllCharacters
                .Where(c => c != null)
                .Where(c => c != selfChar)
                .Where(c => c.data != null)
                .Where(c => !c.data.dead)
            : Character.AllCharacters
                .Where(c => c != null)
                .Where(c => c != selfChar)
                .Where(c => c.data != null)
                .Where(c => !c.data.dead)
                .Where(c => !c.data.isCarried);

        var others = candidates.ToArray();
        if (others.Length == 0)
        {
            Debug.Log("Tried to swap, but no other player found");
            return;
        }

        var targetCharacter = others[UnityEngine.Random.Range(0, others.Length)];

        if (JellyJamLocationSwapPEAKMod.SwapDamageConfig.DropCarryOnTeleport.Value)
        {
            if (selfChar.data.IsCarryingCharacter)
                selfChar.refs.carriying.Drop(selfChar.data.carriedPlayer);
            if (targetCharacter.data.IsCarryingCharacter)
                targetCharacter.refs.carriying.Drop(targetCharacter.data.carriedPlayer);
        }

        SwapWith(selfChar, targetCharacter);

        // Update cooldown on successful swap
        lastSwapTime = Time.time;
    }

    private void SwapWith(Character selfChar, Character target)
    {
        if (selfChar == null || target == null) return;
        if (selfChar.data.dead || target.data.dead) return;

        Vector3 selfPos = selfChar.Center;
        Vector3 targetPos = target.Center;

        selfChar.photonView.RPC("WarpPlayerRPC", RpcTarget.All, targetPos, false);
        target.photonView.RPC("WarpPlayerRPC", RpcTarget.All, selfPos, false);

        Debug.Log($"[RandomDamageTeleport] Swapped {selfChar.name} with {target.name}");
    }
}
