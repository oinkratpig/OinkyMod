﻿using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Reflection;
using System;

namespace OinkyMod
{
    public static class Patches
    {
        /// <summary>
        /// Adds custom audio for the boombox.
        /// </summary>
        [HarmonyPatch(typeof(BoomboxItem), "Start")]
        [HarmonyPostfix]
        public static void BoomboxCustomAudio(BoomboxItem __instance)
        {
            if(ModConfig.CustomBoomboxMusic.Count > 0)
                __instance.musicAudios = ModConfig.CustomBoomboxMusic.ToArray();

        } // end BoomboxCustomAudio

        /// <summary>
        /// Increases quota day count.
        /// </summary>
        [HarmonyPatch(typeof(TimeOfDay), "Awake")]
        [HarmonyPostfix]
        public static void IncreasedQuotaDays(TimeOfDay __instance)
        {
            if(ModConfig.CheatsEnabled)
                __instance.quotaVariables.deadlineDaysAmount = ModConfig.QuotaDays;

        } // end IncreasedQuotaDays

        /// <summary>
        /// Increases stamina regeneration
        /// </summary>
        [HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
        [HarmonyPrefix]
        public static void FlatStaminaRegain(PlayerControllerB __instance)
        {
            if(ModConfig.CheatsEnabled && !__instance.isSprinting)
                __instance.sprintMeter = Mathf.Clamp(__instance.sprintMeter + Time.deltaTime / ModConfig.BonusStaminaRegain, 0f, 1f);

        } // end FlatStaminaRegain

        /// <summary>
        /// Disable normal scrollwheel scrolling if inverted.
        /// </summary>
        [HarmonyPatch(typeof(PlayerControllerB), "SwitchItem_performed")]
        [HarmonyPrefix]
        public static bool DisableNormalScrollWheel()
        {
            if(ModConfig.InvertMousewheel)
                return false;
            return true;

        } // end DisableNormalScrollWheel

        /// <summary>
        /// Disable default key for emoting.
        /// </summary>
        [HarmonyPatch(typeof(PlayerControllerB), "Emote1_performed")]
        [HarmonyPrefix]
        public static bool DisableEmote1() { return false; }

        /// <summary>
        /// Disable default key for emoting.
        /// </summary>
        [HarmonyPatch(typeof(PlayerControllerB), "Emote2_performed")]
        [HarmonyPrefix]
        public static bool DisableEmote2() { return false; }


    } // end class Patches

} // end namespace