using BepInEx;
using System.IO;

namespace OinkyMod
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            Directory.CreateDirectory(Mod.ModFolder);
            Logging.Logger = Logger;
            Mod.LoadCustomMusic();
            Mod.harmonyInst.PatchAll(typeof(Patches));

            Logging.Write($"{PluginInfo.PLUGIN_GUID} is loaded.");

        } // end Awake

    } // end class Plugin

} // end namespace