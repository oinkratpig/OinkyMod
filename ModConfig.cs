using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace OinkyMod
{
    internal static class ModConfig
    {
        /// <summary>
        /// Cooldown modes for switching items using number keys.<br/>
        /// </summary>
        public enum SwitchCooldownModes
        {
            /// <summary>
            /// No cooldown.
            /// </summary>
            NoCooldown,
            /// <summary>
            /// Cooldown only when switching.
            /// </summary>
            SingleCooldown,
            /// <summary>
            /// Longer cooldown for distance scrolled.
            /// </summary>
            ScrollCooldown
        }

        /// <summary>
        /// OinkyMod's folder for holding logs, config, audio, etc.
        /// </summary>
        public static string ModFolder { get; private set; }

        /// <summary>
        /// List of all custom boombox <see cref="AudioClip"/>s.
        /// </summary>
        public static List<AudioClip> CustomBoomboxMusic { get; set; }

        /// <summary>
        /// Custom amount of days to meet quota.
        /// </summary>
        public static int QuotaDays { get; private set; }

        /// <summary>
        /// Flat amount of stamina regain to always recover when not sprinting.<br/>
        /// 20f = only noticeable when heavy<br/>
        /// 15f = a bit faster (balanced)<br/>
        /// 10f = a lot faster (cheating)
        /// </summary>
        public static float BonusStaminaRegain { get; private set; }

        /// <summary>
        /// Invert mousewheel item switching direction
        /// </summary>
        public static bool InvertMousewheel { get; private set; }

        /// <summary>
        /// Whether changes that provide advantages will be enabled.
        /// </summary>
        public static bool CheatsEnabled { get; private set; }

        /// <summary>
        /// Cooldown mode for switching items using number keys.
        /// </summary>
        public static SwitchCooldownModes SwitchCooldownMode;

        // Constructor
        static ModConfig()
        {
            ModFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), PluginInfo.PLUGIN_NAME);

            SwitchCooldownMode = SwitchCooldownModes.SingleCooldown;
            InvertMousewheel = true;

            CheatsEnabled = false;
            QuotaDays = 4;
            BonusStaminaRegain = 0f;

        } // end constructor

    } // end class ModConfig

} // end namespace