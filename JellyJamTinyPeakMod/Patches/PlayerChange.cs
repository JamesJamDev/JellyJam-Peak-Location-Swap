using HarmonyLib;
using UnityEngine;
using static CharacterAfflictions;

namespace JellyJamTinyPeakMod.Patches
{
    [HarmonyPatch(typeof(Character))]
    public class PlayerPatch
    {
        static float bodySize = 0.36f;

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void UpdatePostfix(Character __instance)
        {
            if (__instance != null)
            {
                __instance.transform.localScale = Vector3.one * bodySize;
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                Character[] allCharacters = Object.FindObjectsByType<Character>(FindObjectsSortMode.None);

                foreach (Character c in allCharacters)
                {
                    c.transform.localScale = Vector3.one * bodySize;
                }
            }

        }
    }

    [HarmonyPatch(typeof(CharacterAfflictions), "UpdateWeight")]
    public class UpdateWeightPatch
    {
        static float bodySize = 0.36f;
        [HarmonyPostfix]
        private static void Postfix(CharacterAfflictions __instance)
        {
            var character = __instance.character;
            if (character == null) return;


            // Take the original value
            float originalWeightStatus = __instance.GetCurrentStatus(CharacterAfflictions.STATUSTYPE.Weight);

            // Multiply it
            float increasedWeightStatus = originalWeightStatus * 1.2f;
            
            // Set it as the new value
            __instance.SetStatus(CharacterAfflictions.STATUSTYPE.Weight, increasedWeightStatus);

            Debug.Log($"[TINY MOD] Increased weight status from {originalWeightStatus:F3} to {increasedWeightStatus:F3}");
        }
    }


    [HarmonyPatch(typeof(CharacterMovement))]
    public class MovementPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("Update")]
        static void UpdatePostfix(CharacterMovement __instance)
        {
            __instance.movementModifier = 0.8f;
            __instance.jumpGravity = 3f;  // 10f is default


        }
    }

    // Prevent Fall Damage
    [HarmonyPatch(typeof(CharacterMovement), "CheckFallDamage")]
    public class FallDamagePatch
    {
        static bool Prefix(CharacterMovement __instance)
        {
            Character character = __instance.GetComponent<Character>();
            if (character == null) return false;

            float fallTime = AccessTools.Method(typeof(CharacterMovement), "FallTime").Invoke(__instance, null) as float? ?? 0f;
            if (fallTime < 1.5f)
                return false;

            Debug.Log($"[TINY MOD] Fall Damage Prevented");

            // Returning false prevents original method from running.
            return false;
        }
    }



    [HarmonyPatch(typeof(CharacterClimbing))]
    public class CharacterClimbingPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        static void Postfix(CharacterClimbing __instance)
        {
            __instance.climbSpeedMod = 1.3f;   // Multiply climb speed
        }
    }


}
