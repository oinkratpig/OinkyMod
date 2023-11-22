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

        /// <summary>
        /// Callback for switching slots with mousewheel (inverted).
        /// </summary>
        public static InvertedMousewheelSwitchCallback InvertedMousewheelSwitch { get; set; }
        public delegate void InvertedMousewheelSwitchCallback(bool forward);

        private static List<InputAction> _actions;

        // Constructor
        static CustomKeys()
        {
            _actions = new List<InputAction>();

            // Debug
            InputAction debug = new InputAction();
            debug.AddBinding("<Keyboard>/0");
            debug.started += Debug;
            _actions.Add(debug);

            // Number keys
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

            // Invert mousewheel
            if(ModConfig.InvertMousewheel)
            {
                InputAction switchItem = new InputAction();
                switchItem.AddBinding("<Mouse>/scroll/y");
                switchItem.performed += SwitchMousewheelInverted;
                _actions.Add(switchItem);
            }

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

        // Debug key
        // Add things in body for debug - key is '0'
        private static void Debug(InputAction.CallbackContext context) { }

        // Switch slots with number keys
        private static void SwitchToSlot1(InputAction.CallbackContext context) { SwitchToSlot(0); }
        private static void SwitchToSlot2(InputAction.CallbackContext context) { SwitchToSlot(1); }
        private static void SwitchToSlot3(InputAction.CallbackContext context) { SwitchToSlot(2); }
        private static void SwitchToSlot4(InputAction.CallbackContext context) { SwitchToSlot(3); }

        // Inverted mousewheel switch
        private static void SwitchMousewheelInverted(InputAction.CallbackContext context)
        {
            InvertedMousewheelSwitch(context.ReadValue<float>() > 0f);

        } // end SwitchMousewheelInverted

    } // end class CustomKeys

} // end namespace