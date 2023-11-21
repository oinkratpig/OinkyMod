using DunGen;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;
using System.Windows.Input;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.InputSystem.Controls;
using System.Reflection;
using System;

namespace OinkyMod
{
    public static class Patches
    {
        private static float _stamina;
        private static int _switchToSlot = -1;
        private static List<InputAction> _actions;
        private static PlayerControllerB _playerBeingControlled;

        // Constructor
        static Patches()
        {
            _actions = new List<InputAction>();
            InputAction alpha1 = new InputAction();
            alpha1.AddBinding("<Keyboard>/1");
            alpha1.started += SwitchToSlot1;
            _actions.Add(alpha1);
            InputAction alpha2 = new InputAction();
            alpha2.AddBinding("<Keyboard>/2");
            alpha2.started += SwitchToSlot2;
            _actions.Add(alpha2);
            InputAction alpha3 = new InputAction();
            alpha3.AddBinding("<Keyboard>/3");
            alpha3.started += SwitchToSlot3;
            _actions.Add(alpha3);
            InputAction alpha4 = new InputAction();
            alpha4.AddBinding("<Keyboard>/4");
            alpha4.started += SwitchToSlot4;
            _actions.Add(alpha4);

        } // end constructor

        /// <summary>
        /// Adds custom audio for the boombox.
        /// </summary>
        [HarmonyPatch(typeof(BoomboxItem), "Start")]
        [HarmonyPostfix]
        public static void BoomboxCustomAudio(BoomboxItem __instance)
        {
            if(Mod.CustomBoomboxMusic.Count > 0)
                __instance.musicAudios = Mod.CustomBoomboxMusic.ToArray();

        } // end BoomboxCustomAudio

        /// <summary>
        /// Increases quota day count.
        /// </summary>
        [HarmonyPatch(typeof(TimeOfDay), "Awake")]
        [HarmonyPostfix]
        public static void IncreasedQuotaDays(TimeOfDay __instance)
        {
            __instance.quotaVariables.deadlineDaysAmount = Mod.NewQuotaDays;

        } // end IncreasedQuotaDays

        [HarmonyPatch(typeof(PlayerControllerB), "Start")]
        [HarmonyPostfix]
        public static void PlayerStart(PlayerControllerB __instance)
        {
            if(__instance.IsOwner)
                _playerBeingControlled = __instance;

        } // end PlayerStart

        /// <summary>
        /// Increases stamina regeneration
        /// </summary>
        [HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
        [HarmonyPrefix]
        public static void FlatStaminaRegain(PlayerControllerB __instance)
        {
            if(!__instance.isSprinting)
                __instance.sprintMeter = Mathf.Clamp(__instance.sprintMeter + Time.deltaTime / Mod.BonusStaminaRegain, 0f, 1f);

        } // end FlatStaminaRegain

        /// <summary>
        /// Registers new keys for the player.
        /// </summary>
        [HarmonyPatch(typeof(PlayerControllerB), "OnEnable")]
        [HarmonyPrefix]
        public static void PlayerRegisterKeys(PlayerControllerB __instance)
        {
            foreach (InputAction action in _actions)
                action.Enable();

        } // end PlayerRegisterKeys

        /// <summary>
        /// Deregisters new keys for the player.
        /// </summary>
        [HarmonyPatch(typeof(PlayerControllerB), "OnDisable")]
        [HarmonyPrefix]
        public static void PlayerDeregisterKeys(PlayerControllerB __instance)
        {
            foreach (InputAction action in _actions)
                action.Disable();

        } // end PlayerDeregisterKeys

        // Bad way of doing it but I'm tired gimme a break D:
        private static void SwitchToSlot1(InputAction.CallbackContext obj) { SwitchToSlot(0); }
        private static void SwitchToSlot2(InputAction.CallbackContext obj) { SwitchToSlot(1); }
        private static void SwitchToSlot3(InputAction.CallbackContext obj) { SwitchToSlot(2); }
        private static void SwitchToSlot4(InputAction.CallbackContext obj) { SwitchToSlot(3); }

        /// <summary>
        /// Switch to a specific slot
        /// </summary>
        private static void SwitchToSlot(int slot)
        {
            Traverse timeSinceSwitchingSlots = Traverse.Create(_playerBeingControlled).Field("timeSinceSwitchingSlots");

            // Wait cooldown
            float cooldownLength = 0.3f;
            if (Mod.switchCooldownMode != Mod.SwitchCooldownMode.NoCooldown &&
                timeSinceSwitchingSlots.GetValue<float>() < cooldownLength)
            {
                return;
            }

            // Determine inventory scrolling direction
            var countWrap = (int from, int to, int min, int max, bool positiveIncrement) =>
            {
                int moves = 0;
                while(from != to)
                {
                    int increment = (positiveIncrement) ? 1 : -1;
                    moves += increment;
                    from += increment;
                    if (from > max) from = min;
                    else if (from < min) from = max;
                }
                return moves;
            };
            int distanceRight = countWrap(_playerBeingControlled.currentItemSlot, slot, 0, _playerBeingControlled.ItemSlots.Length, true);
            int distanceLeft = countWrap(_playerBeingControlled.currentItemSlot, slot, 0, _playerBeingControlled.ItemSlots.Length, false);
            bool forward = Math.Abs(distanceRight) > Math.Abs(distanceLeft);
            
            // Scroll inventory appropriate number of times
            MethodInfo switchToItemSlot = _playerBeingControlled.GetType().GetMethod("SwitchToItemSlot", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo switchItemSlotsServerRpc = _playerBeingControlled.GetType().GetMethod("SwitchItemSlotsServerRpc", BindingFlags.NonPublic | BindingFlags.Instance);
            while (_playerBeingControlled.currentItemSlot != slot)
            {
                int nextSlot = Traverse.Create(_playerBeingControlled).Method("NextItemSlot", new Type[] { typeof(bool) }).GetValue<int>(new object[] { forward });
                switchToItemSlot.Invoke(_playerBeingControlled, new object[] { nextSlot, null });
                switchItemSlotsServerRpc.Invoke(_playerBeingControlled, new object[] { forward });
            }
            if (_playerBeingControlled.currentlyHeldObjectServer != null)
            {
                _playerBeingControlled.currentlyHeldObjectServer.gameObject.GetComponent<AudioSource>().PlayOneShot(_playerBeingControlled.currentlyHeldObjectServer.itemProperties.grabSFX, 0.6f);
            }
            Traverse.Create(_playerBeingControlled).Field("timeSinceSwitchingSlots").SetValue(0f);

            // Set cooldown
            if (Mod.switchCooldownMode == Mod.SwitchCooldownMode.SingleCooldown)
                timeSinceSwitchingSlots.SetValue(0f);
            else if (Mod.switchCooldownMode == Mod.SwitchCooldownMode.ScrollCooldown)
            {
                Logging.Write($"distanceLeft: {distanceLeft}, distanceRight: {distanceRight}");
                timeSinceSwitchingSlots.SetValue(-cooldownLength * Math.Max(Math.Min(Math.Abs(distanceLeft), Math.Abs(distanceRight)) - 1, 0));
                Logging.Write($"cooldown: {timeSinceSwitchingSlots.GetValue<float>()}");
            }

        } // end SwitchToSlot

        /// <summary>
        /// Handle player input
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
        [HarmonyPostfix]
        public static void PlayerLateUpdate(PlayerControllerB __instance)
        {
            // Switch to new item slot
            if(_switchToSlot != -1)
            {
                // Traverse isn't working for some reason?
                MethodInfo info = __instance.GetType().GetMethod("SwitchToItemSlot", BindingFlags.NonPublic | BindingFlags.Instance);
                info.Invoke(__instance, new object[] { _switchToSlot, null});

                _switchToSlot = -1;
            }

        } // end PlayerLateUpdate

        /// <summary>
        /// Switches items based on the number key press.
        /// </summary>
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