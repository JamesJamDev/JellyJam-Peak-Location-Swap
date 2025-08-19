using HarmonyLib;
using UnityEngine;

namespace JellyJamTinyPeakMod.Patches
{
    [HarmonyPatch(typeof(BreakableBridge), "Update")]
    internal class BridgePatch
    {
        [HarmonyPostfix]
        static void UpdatePostFix(BreakableBridge __instance)
        {
            // Attempt to reduce the chance of breaking to 0
            if (__instance.breakChance > 0)
            {
                __instance.breakChance = 0;
            }
        }
    }
}
