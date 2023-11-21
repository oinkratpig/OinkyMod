using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.InputSystem;

namespace OinkyMod
{
    public static class CustomKeys
    {
        
        /// <summary>
        /// Callback for switching slots with number keys.
        /// </summary>
        public static SwitchToSlotCallback SwitchToSlot { get; set; }
        public delegate void SwitchToSlotCallback(int slot);

        private static List<InputAction> _actions;

        // Constructor
        static CustomKeys()
        {
            // Number keys
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

        } // end CustomKeys

        /// <summary>
        /// Registers new keys for the player.
        /// </summary>
        [HarmonyPatch(typeof(PlayerControllerB), "OnEnable")]
        [HarmonyPrefix]
        public static void PlayerRegisterKeys(PlayerControllerB __instance)
        {
            Logging.Write("Registering custom keys.");
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
            Logging.Write("Deregistering custom keys.");
            foreach (InputAction action in _actions)
                action.Disable();

        } // end PlayerDeregisterKeys

        // Bad way of doing it but I'm tired gimme a break D:
        private static void SwitchToSlot1(InputAction.CallbackContext obj) { SwitchToSlot(0); }
        private static void SwitchToSlot2(InputAction.CallbackContext obj) { SwitchToSlot(1); }
        private static void SwitchToSlot3(InputAction.CallbackContext obj) { SwitchToSlot(2); }
        private static void SwitchToSlot4(InputAction.CallbackContext obj) { SwitchToSlot(3); }

    } // end class CustomKeys

} // end namespace