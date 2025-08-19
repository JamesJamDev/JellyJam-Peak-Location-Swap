using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace JellyJamTinyPeakMod.Patches
{
    [HarmonyPatch(typeof(BackpackData))]
    public class BackpackPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("AddItem")]
        public static bool PreventAddingToFourthSlot(ref byte backpackSlotID)
        {
            if (backpackSlotID == 3)
            {
                Debug.Log("[TinyMod] Prevented adding item to disabled 4th backpack slot.");
                return false; // skip the original AddItem method
            }
            return true; 
        }

        [HarmonyPatch(typeof(BackpackWheelSlice), "UpdateInteractable")]
        class Patch_BackpackWheelSlice_UpdateInteractable
        {
            static void Postfix(BackpackWheelSlice __instance)
            {
                if (__instance.backpackSlot == 3)
                {
                    __instance.button.interactable = false;
                }
            }
        }

        [HarmonyPatch(typeof(BackpackWheelSlice), "Hover")]
        class Patch_BackpackWheelSlice_Hover
        {
            static bool Prefix(BackpackWheelSlice __instance)
            {
                return __instance.backpackSlot != 3;
            }
        }


    }
}
