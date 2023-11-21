using DunGen;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;
using System.Windows.Input;
using UnityEngine.InputSystem;

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
            __instance.quotaVariables.deadlineDaysAmount = Mod.NewQuotaDays;

        } // end IncreasedQuotaDays

        [HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
        [HarmonyPrefix]
        public static void FlatStaminaRegain(PlayerControllerB __instance)
        {
            if(!__instance.isSprinting)
                __instance.sprintMeter = Mathf.Clamp(__instance.sprintMeter + Time.deltaTime / Mod.BonusStaminaRegain, 0f, 1f);

        } // end FlatStaminaRegain

        [HarmonyPatch(typeof(PlayerControllerB), "OnEnable")]
        [HarmonyPrefix]
        public static void PlayerRegisterKeys(PlayerControllerB __instance)
        {
            InputAction action = new InputAction();
            action.AddBinding("<Keyboard>/r");
            action.performed += Test;

        } // end PlayerRegisterKeys

        public static void Test(InputAction.CallbackContext context)
        {
            Logging.Write("test!!!!!");

        } // end Test

        [HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
        [HarmonyPostfix]
        public static void PlayerNumKeySwitch(PlayerControllerB __instance)
        {
            /*
            if (Input.GetKeyDown(KeyCode.Alpha3))
                Traverse.Create(__instance).Method("SwitchToItemSlot", 1, null).GetValue();
            else if (Input.GetKeyDown(KeyCode.Alpha4))
                Traverse.Create(__instance).Method("SwitchToItemSlot", 2, null).GetValue();
            else if(Input.GetKeyDown(KeyCode.Alpha5))
                Traverse.Create(__instance).Method("SwitchToItemSlot", 3, null).GetValue();
            else if(Input.GetKeyDown(KeyCode.Alpha6))
                Traverse.Create(__instance).Method("SwitchToItemSlot", 4, null).GetValue();
            */

        } // end PlayerNumKeySwitch

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