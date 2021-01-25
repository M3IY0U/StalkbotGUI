using System.IO;
using Newtonsoft.Json;

namespace StalkbotGUI.Stalkbot.Utilities
{
    /// <summary>
    /// Config data class
    /// </summary>
    public sealed class Config
    {
        // Discord
        public string Token { get; set; } = "changeme";
        public string Prefix { get; set; } = "change!";
        // Webcam
        public bool CamEnabled { get; set; } = false;
        public int CamTimer { get; set; } = 3000;
        public int CamWidth { get; set; } = 1280;
        public int CamHeight { get; set; } = 720;
        public int DefaultCam { get; set; } = 0;
        // Screenshot
        public bool SsEnabled { get; set; } = false;
        public double BlurAmount { get; set; } = 1;
        // Sounds
        public bool TtsEnabled { get; set; } = false;
        public bool PlayEnabled { get; set; } = false;
        public double Timeout { get; set; } = 10000;
        // Misc
        public bool ProcessesEnabled { get; set; } = false;
        public string FolderPath { get; set; } = "";
        public bool ClipboardEnabled { get; set; } = false;
        public bool AutoStartDiscord { get; set; } = false;
        public bool MinimizeToTray { get; set; } = false;

        // Actual Config 
        private static Config _instance;

        // Singleton pattern
        public static Config Instance
        {
            get
            {
                if (_instance == null)
                    LoadConfig();
                return _instance;
            }
        }

        /// <summary>
        /// Returns the config in a neatly formatted way
        /// </summary>
        /// <returns>A string of relevant config settings</returns>
        public override string ToString()
            =>  "```\n" +
                $"Webcam => {CamEnabled}\n" +
                $"Default Cam => {DefaultCam} ({Constants.Cameras[DefaultCam].Name})\n" +
                $"Attempted Cam Resolution => {CamWidth} x {CamHeight}\n" +
                $"Screenshot => {SsEnabled}\n" +
                $"Screenshot Blur => {BlurAmount}\n" +
                $"Play => {PlayEnabled}\n" +
                $"TTS => {TtsEnabled}\n" +
                $"Play/TTS Timeout => {Timeout}\n" +
                $"Processes => {ProcessesEnabled}\n" +
                $"Clipboard => {ClipboardEnabled}\n" +
                $"Folder => {FolderPath}\n" +
                $"```";

        /// <summary>
        /// Reloads the config by calling the constructor again
        /// </summary>
        public void ReloadConfig()
            => _instance = new Config();

        /// <summary>
        /// Writes current config to file
        /// </summary>
        public void SaveConfig()
            => File.WriteAllText("config.json", JsonConvert.SerializeObject(this, Formatting.Indented));

        /// <summary>
        /// Load config from file or creates a default one
        /// </summary>
        public static void LoadConfig()
            => _instance = File.Exists("config.json")
                ? JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"))
                : new Config();

        /// <summary>
        /// Converts a string to the corresponding toggle
        /// </summary>
        /// <param name="command">The string to convert</param>
        /// <returns>The correct toggle for the string</returns>
        public bool IsEnabled(string command)
        {
            switch (command)
            {
                case "webcam":
                case "webcams":
                    return CamEnabled;
                case "play":
                    return PlayEnabled;
                case "screenshot":
                    return SsEnabled;
                case "tts":
                    return TtsEnabled;
                case "folder":
                    return !string.IsNullOrEmpty(FolderPath);
                case "proc":
                case "processes":
                    return ProcessesEnabled;
                case "clipboard":
                    return ClipboardEnabled;
                default:
                    return false;
            }
        }
    }
}