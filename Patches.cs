using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OinkyMod
{
    public static class Patches
    {
        private static float _stamina;

        [HarmonyPatch(typeof(BoomboxItem), "Start")]
        [HarmonyPostfix]
        public static void BoomboxCustomAudio(BoomboxItem __instance)
        {
            if(Mod.CustomBoomboxMusic.Count > 0)
                __instance.musicAudios = Mod.CustomBoomboxMusic.ToArray();

        } // end BoomboxCustomAudio

        [HarmonyPatch(typeof(TimeOfDay), "Awake")]
        [HarmonyPostfix]
        public static void IncreasedQuotaDays(TimeOfDay __instance)
        {
            __instance.quotaVariables.deadlineDaysAmount = NewQuotaDays;

        } // end IncreasedQuotaDays

        [HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
        [HarmonyPrefix]
        public static void FlatStaminaRegain(PlayerControllerB __instance)
        {
            if(!__instance.isSprinting)
                __instance.sprintMeter = Mathf.Clamp(__instance.sprintMeter + Time.deltaTime / Mod.BonusStaminaRegain, 0f, 1f);
            
        } // end StaminaPrefix

        /*
        [HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
        [HarmonyPrefix]
        public static void StaminaPrefix(PlayerControllerB __instance)
        {
            _stamina = __instance.sprintMeter;

        } // end StaminaPrefix
        */

        /*
        [HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
        [HarmonyPostfix]
        public static void StaminaPostfix(PlayerControllerB __instance)
        {
            if (__instance.sprintMeter > _stamina)
            {
                Logging.Write($"Higher: {_stamina}");
                _stamina = Mathf.Clamp(_stamina + (__instance.sprintMeter - _stamina) * Mod.StaminaRegenMultiplier, 0f, 1f);
                Logging.Write($"to {_stamina}");
            }
            else Logging.Write($"Lower: {_stamina}");

        } // end StaminaPrefix
        */

    } // end class Patches

} // end namespace