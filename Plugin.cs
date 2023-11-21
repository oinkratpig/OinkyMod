using BepInEx;
using HarmonyLib;
using System.IO;

namespace OinkyMod
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private static Harmony _harmony = new Harmony(PluginInfo.PLUGIN_GUID);

        private void Awake()
        {
            Directory.CreateDirectory(ModConfig.ModFolder);
            Logging.Logger = Logger;
            Mod.LoadCustomMusic();
            _harmony.PatchAll(typeof(Patches));
            _harmony.PatchAll(typeof(Player));
            _harmony.PatchAll(typeof(CustomKeys));

            Logging.Write($"{PluginInfo.PLUGIN_GUID} is loaded.");

        } // end Awake

    } // end class Plugin

} // end namespace