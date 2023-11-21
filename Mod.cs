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
        /// Load all custom music.
        /// </summary>
        public static async void LoadCustomMusic()
        {
            Logging.Write("Loading custom boombox music...");
            ModConfig.CustomBoomboxMusic = new List<AudioClip>();
            string path;
            int i = 1;
            while (true)
            {
                string filename = $"boombox{i}.ogg";
                path = Path.Combine(ModConfig.ModFolder, filename);

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
                        ModConfig.CustomBoomboxMusic.Add(clip);
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

        } // end GetCustomAudioClip

    } // end class Mod

} // end namespace