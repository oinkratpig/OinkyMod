using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace OinkyMod
{
    public static class Player
    {
        public static PlayerControllerB PlayerBeingControlled { get; private set; }

        // Constructor
        static Player()
        {
            CustomKeys.SwitchToSlot = SwitchToSlot;

        } // end Player

        [HarmonyPatch(typeof(PlayerControllerB), "Start")]
        [HarmonyPostfix]
        public static void PlayerStart(PlayerControllerB __instance)
        {
            if (__instance.IsOwner)
                PlayerBeingControlled = __instance;

        } // end PlayerStart

        /// <summary>
        /// Switch to a specific slot
        /// </summary>
        private static void SwitchToSlot(int slot)
        {
            Traverse timeSinceSwitchingSlots = Traverse.Create(PlayerBeingControlled).Field("timeSinceSwitchingSlots");

            // Prevent switching under certain conditions
            // (yes, the dev actually has this many conditionals)
            // (at least in dnSpy)
            bool throwingObject = Traverse.Create(PlayerBeingControlled).Field("throwingObject").GetValue<bool>();
            if(PlayerBeingControlled.inTerminalMenu || PlayerBeingControlled.jetpackControls || PlayerBeingControlled.disablingJetpackControls ||
                ((!PlayerBeingControlled.IsOwner || !PlayerBeingControlled.isPlayerControlled || (PlayerBeingControlled.IsServer && !PlayerBeingControlled.isHostPlayerObject)) && !PlayerBeingControlled.isTestingPlayer) ||
                PlayerBeingControlled.isGrabbingObjectAnimation || PlayerBeingControlled.inSpecialInteractAnimation || throwingObject || PlayerBeingControlled.isTypingChat || PlayerBeingControlled.twoHanded || PlayerBeingControlled.activatingItem)
            {
                return;
            }

            // Wait cooldown
            float cooldownLength = 0.3f;
            if (ModConfig.SwitchCooldownMode != ModConfig.SwitchCooldownModes.NoCooldown &&
                timeSinceSwitchingSlots.GetValue<float>() < cooldownLength)
            {
                return;
            }

            // Determine inventory scrolling direction
            var countWrap = (int from, int to, int min, int max, bool positiveIncrement) =>
            {
                int moves = 0;
                while (from != to)
                {
                    int increment = (positiveIncrement) ? 1 : -1;
                    moves += increment;
                    from += increment;
                    if (from > max) from = min;
                    else if (from < min) from = max;
                }
                return moves;
            };
            int distanceRight = countWrap(PlayerBeingControlled.currentItemSlot, slot, 0, PlayerBeingControlled.ItemSlots.Length, true);
            int distanceLeft = countWrap(PlayerBeingControlled.currentItemSlot, slot, 0, PlayerBeingControlled.ItemSlots.Length, false);
            bool forward = Math.Abs(distanceRight) > Math.Abs(distanceLeft);

            // Scroll inventory appropriate number of times
            MethodInfo switchToItemSlot = PlayerBeingControlled.GetType().GetMethod("SwitchToItemSlot", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo switchItemSlotsServerRpc = PlayerBeingControlled.GetType().GetMethod("SwitchItemSlotsServerRpc", BindingFlags.NonPublic | BindingFlags.Instance);
            while (PlayerBeingControlled.currentItemSlot != slot)
            {
                int nextSlot = Traverse.Create(PlayerBeingControlled).Method("NextItemSlot", new Type[] { typeof(bool) }).GetValue<int>(new object[] { forward });
                switchToItemSlot.Invoke(PlayerBeingControlled, new object[] { nextSlot, null });
                switchItemSlotsServerRpc.Invoke(PlayerBeingControlled, new object[] { forward });
            }
            if (PlayerBeingControlled.currentlyHeldObjectServer != null)
            {
                PlayerBeingControlled.currentlyHeldObjectServer.gameObject.GetComponent<AudioSource>().PlayOneShot(PlayerBeingControlled.currentlyHeldObjectServer.itemProperties.grabSFX, 0.6f);
            }
            Traverse.Create(PlayerBeingControlled).Field("timeSinceSwitchingSlots").SetValue(0f);

            // Set cooldown
            if (ModConfig.SwitchCooldownMode == ModConfig.SwitchCooldownModes.SingleCooldown)
                timeSinceSwitchingSlots.SetValue(0f);
            else if (ModConfig.SwitchCooldownMode == ModConfig.SwitchCooldownModes.ScrollCooldown)
            {
                Logging.Write($"distanceLeft: {distanceLeft}, distanceRight: {distanceRight}");
                timeSinceSwitchingSlots.SetValue(-cooldownLength * Math.Max(Math.Min(Math.Abs(distanceLeft), Math.Abs(distanceRight)) - 1, 0));
                Logging.Write($"cooldown: {timeSinceSwitchingSlots.GetValue<float>()}");
            }

        } // end SwitchToSlot

    } // end class Player

} // end namespace