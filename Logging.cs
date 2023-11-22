using BepInEx.Logging;
using System.IO;

namespace OinkyMod
{
    internal static class Logging
    {
        public static ManualLogSource Logger { get; set; }

        private static string _logPath;
        private const string LOG_FILE_NAME = "oinkymod.log";

        // Constructor
        static Logging()
        {
            _logPath = Path.Combine(ModConfig.ModFolder, LOG_FILE_NAME);
            CreateNewLog();

        } // end constructor

        /// <summary>
        /// Creates a new empty log file.
        /// </summary>
        public static void CreateNewLog()
        {
            FileStream newFile = File.Create(_logPath);
            newFile.Close();

        } // end Append

        /// <summary>
        /// Logs a new string.
        /// </summary>
        public static void Write(string text)
        {
            Logger.LogInfo(text);
            using (StreamWriter writer = File.AppendText(_logPath))
                writer.WriteLine(text);

        } // end Write

    } // end class Logging

} // end namespace