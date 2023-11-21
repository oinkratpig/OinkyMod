using HarmonyLib;
using System.Diagnostics;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine;
using System.Collections.Generic;

namespace OinkyMod
{
    public static class Mod
    {
        /// <summary>
        /// OinkyMod's folder for holding logs, config, audio, etc.
        /// </summary>
        public static string ModFolder { get; private set; }

        /// <summary>
        /// Name of the mod folder. Is NOT the entire path.
        /// </summary>
        private const string MOD_FOLDER_NAME = PluginInfo.PLUGIN_NAME;

        /// <summary>
        /// Harmony instance
        /// </summary>
        public static Harmony harmonyInst = new Harmony(PluginInfo.PLUGIN_GUID);

        /// <summary>
        /// List of all custom boombox <see cref="AudioClip"/>s.
        /// </summary>
        public static List<AudioClip> CustomBoomboxMusic { get; private set; }

        /// <summary>
        /// Flat amount of stamina regain to always recover when not sprinting.<br/>
        /// 12f = not much faster (barely noticeable)<br/>
        /// 9f = a bit faster (not too strong)<br/>
        /// 6f = a lot faster (cheating)
        /// </summary>
        public static float BonusStaminaRegain { get; private set; }

        /// <summary>
        /// New amount of days to meet quota.
        /// </summary>
        public static int NewQuotaDays { get; private set; }

        // Constructor
        static Mod()
        {
            NewQuotaDays = 4;
            BonusStaminaRegain = 9f;
            ModFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), MOD_FOLDER_NAME);

        } // end constructor

        /// <summary>
        /// Load all custom music.
        /// </summary>
        public static async void LoadCustomMusic()
        {
            Logging.Write("Loading custom boombox music...");
            CustomBoomboxMusic = new List<AudioClip>();
            string path;
            int i = 1;
            while (true)
            {
                string filename = $"boombox{i}.ogg";
                path = Path.Combine(ModFolder, filename);

                Logging.Write($"Looking for {filename}.");
                if (!File.Exists(path))
                {
                    Logging.Write("End of audio clip search.");
                    break;
                }
                else
                {
                    Logging.Write($"Found {filename}");
                    AudioClip clip = await GetCustomAudioClip(path);
                    if(clip != null)
                        CustomBoomboxMusic.Add(clip);
                }
                i++;
            }

        } // end LoadCustomMusic

        /// <summary>
        /// Converts a file on disk to an <see cref="AudioClip"/>.
        /// </summary>
        private static async Task<AudioClip> GetCustomAudioClip(string path)
        {
            AudioClip clip = null;
            using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.OGGVORBIS))
            {
                uwr.SendWebRequest();

                // wrap tasks in try/catch, otherwise it'll fail silently
                try
                {
                    while (!uwr.isDone) await Task.Delay(5);

                    if (!uwr.isNetworkError && !uwr.isHttpError)
                    {
                        clip = DownloadHandlerAudioClip.GetContent(uwr);
                    }
                }
                catch (Exception _)
                {
                }
            }
            return clip;
        }

    } // end class Mod

} // end namespace