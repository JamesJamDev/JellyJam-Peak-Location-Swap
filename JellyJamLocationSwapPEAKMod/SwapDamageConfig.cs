// Created by https://github.com/Vegasx and adapted by JellyJam for JellyJam's Tiny Peak Mod

using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace JellyJamLocationSwapPEAKMod
{
    internal static class SwapDamageConfig
    {
        internal static ManualLogSource Logger;

        internal static ConfigEntry<bool> IsEnabled;
        internal static ConfigEntry<bool> TogglePoison;
        internal static ConfigEntry<bool> ToggleHot;
        internal static ConfigEntry<bool> ToggleCold;
        internal static ConfigEntry<bool> ToggleCurse;
        internal static ConfigEntry<bool> ToggleSpores;
        internal static ConfigEntry<bool> ToggleInjury;
        internal static ConfigEntry<bool> ToggleThorns;
        internal static ConfigEntry<bool> ToggleHunger;
        internal static ConfigEntry<bool> ToggleWeight;
        internal static ConfigEntry<bool> ToggleDrowsy;
        internal static ConfigEntry<bool> AllowCarried;
        internal static ConfigEntry<bool> DropCarryOnTeleport;
        internal static ConfigEntry<float> SwapCooldown;
        internal static ConfigEntry<float> MinimumAmount;

        private static Harmony _harmony;

        internal static void Initialize(BaseUnityPlugin plugin)
        {
            // Create a dedicated log source
            Logger = BepInEx.Logging.Logger.CreateLogSource("SwapDamageConfig");
            BindConfig(plugin);
            if (_harmony == null)
            {
                _harmony = new Harmony("JellyJam.LocationSwapMod.Config");
                _harmony.PatchAll(typeof(JellyJamTinyPeakMod.Patches.AfflictionPatch));
            }
            Logger.LogInfo("SwapDamageConfig initialized");
        }

        private static void BindConfig(BaseUnityPlugin plugin)
        {
            IsEnabled = plugin.Config.Bind("General", "Enabled", true, "Mod enabled state.");
            ToggleCurse = plugin.Config.Bind("StatusTypes", "Curse", false, "Should this status cause a teleport?");
            ToggleHunger = plugin.Config.Bind("StatusTypes", "Hunger", false, "Should this status cause a teleport?");
            ToggleWeight = plugin.Config.Bind("StatusTypes", "Weight", false, "Should this status cause a teleport?");
            TogglePoison = plugin.Config.Bind("StatusTypes", "Poison", true, "Should this status cause a teleport?");
            ToggleHot = plugin.Config.Bind("StatusTypes", "Hot", true, "Should this status cause a teleport?");
            ToggleCold = plugin.Config.Bind("StatusTypes", "Cold", true, "Should this status cause a teleport?");
            ToggleSpores = plugin.Config.Bind("StatusTypes", "Spores", true, "Should this status cause a teleport?");
            ToggleInjury = plugin.Config.Bind("StatusTypes", "Injury", true, "Should this status cause a teleport?");
            ToggleThorns = plugin.Config.Bind("StatusTypes", "Thorns", true, "Should this status cause a teleport?");
            ToggleDrowsy = plugin.Config.Bind("StatusTypes", "Drowsy", true, "Should this status cause a teleport?");
            AllowCarried = plugin.Config.Bind("General", "AllowCarriedTeleports", true, "Whether or not carried players are affected by teleporter events.");
            DropCarryOnTeleport = plugin.Config.Bind("General", "DropCarryOnTeleport", true, "Whether or not carried players are dropped when the player holding them is teleported.");
            SwapCooldown = plugin.Config.Bind("General", "SwapCooldown", 2f, "Delay between allowed teleport events per user.");
            MinimumAmount = plugin.Config.Bind("General", "MinimumAmount", 0f, "Minimum amount of status required to trigger a teleport. Smallest tick is 0.025. This Number is divided by 1,000 due to slider size. So 25 = 0.025");
        }
    }
}
