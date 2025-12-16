using HarmonyLib;
using UnityEngine;
using BepInEx;
using System.IO;
using System.Linq;


namespace JellyJamTinyPeakMod.Patches
{
    [HarmonyPatch(typeof(Character))]
    public class AfflictionPatch
    {
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void AwakePostfix(Character __instance)
        {
            if (__instance == null) return;

            // Add the attachment if it doesn't exist
            if (__instance.GetComponent<RandomDamageTeleport>() == null)
                __instance.gameObject.AddComponent<RandomDamageTeleport>();
        }
    }
}
