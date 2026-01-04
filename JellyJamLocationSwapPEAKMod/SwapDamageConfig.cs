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
            IsEnabled = plugin.Config.Bind("General", "GlobalEnable", true, "Mod enabled state.");
            ToggleCurse = plugin.Config.Bind("StatusTypes", "CurseEnabled", false, "Should this status cause a teleport?");
            ToggleHunger = plugin.Config.Bind("StatusTypes", "HungerEnabled", false, "Should this status cause a teleport?");
            ToggleWeight = plugin.Config.Bind("StatusTypes", "WeightEnabled", false, "Should this status cause a teleport?");
            TogglePoison = plugin.Config.Bind("StatusTypes", "PoisonEnabled", true, "Should this status cause a teleport?");
            ToggleHot = plugin.Config.Bind("StatusTypes", "HotEnabled", true, "Should this status cause a teleport?");
            ToggleCold = plugin.Config.Bind("StatusTypes", "ColdEnabled", true, "Should this status cause a teleport?");
            ToggleSpores = plugin.Config.Bind("StatusTypes", "SporesEnabled", true, "Should this status cause a teleport?");
            ToggleInjury = plugin.Config.Bind("StatusTypes", "InjuryEnabled", true, "Should this status cause a teleport?");
            ToggleThorns = plugin.Config.Bind("StatusTypes", "ThornsEnabled", true, "Should this status cause a teleport?");
            ToggleDrowsy = plugin.Config.Bind("StatusTypes", "DrowsyEnabled", true, "Should this status cause a teleport?");
            AllowCarried = plugin.Config.Bind("General", "AllowCarriedTeleports", true, "Whether or not carried players are affected by teleporter events.");
            DropCarryOnTeleport = plugin.Config.Bind("General", "DropCarryOnTeleport", true, "Whether or not carried players are dropped when the player holding them is teleported.");
            SwapCooldown = plugin.Config.Bind("General", "SwapCooldown", 2f, "Delay between allowed teleport events per user.");
            MinimumAmount = plugin.Config.Bind("General", "MinimumAmount", 0f, "Minimum amount of status required to trigger a teleport.");
        }
    }
}
