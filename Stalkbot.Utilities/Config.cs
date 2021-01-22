using System.IO;
using Newtonsoft.Json;

namespace StalkbotGUI.Stalkbot.Utilities
{
    public sealed class Config
    {
        // Discord
        public string Token { get; set; }
        public string Prefix { get; set; }
        // Webcam
        public bool CamEnabled { get; set; }
        public int CamTimer { get; set; }
        public int CamWidth { get; set; }
        public int CamHeight { get; set; }
        public int DefaultCam { get; set; }
        // Screenshot
        public bool SsEnabled { get; set; }
        public double BlurAmount { get; set; }
        // Sounds
        public bool TtsEnabled { get; set; }
        public bool PlayEnabled { get; set; }
        public double Timeout { get; set; }
        // Misc
        public bool ProcessesEnabled { get; set; }
        public string FolderPath { get; set; }
        public bool ClipboardEnabled { get; set; }

        // Actual Config 
        private static Config _instance;

        public static Config Instance
        {
            get
            {
                if(_instance == null)
                    LoadConfig();
                return _instance;
            }
        }

        public void ReloadConfig()
            => _instance = new Config();

        public void SaveConfig()
            => File.WriteAllText("config.json", JsonConvert.SerializeObject(this, Formatting.Indented));

        public static void LoadConfig()
            => _instance = File.Exists("config.json")
                ? JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json")) 
                : new Config();

        public bool IsEnabled(string command)
        {
            switch (command)
            {
                case "webcam":
                    return CamEnabled;
                case "play":
                    return PlayEnabled;
                case "screenshot":
                    return SsEnabled;
                case "tts":
                    return TtsEnabled;
                case "folder":
                    return string.IsNullOrEmpty(FolderPath);
                case "proc":
                    return ProcessesEnabled;
                case "clipboard":
                    return ClipboardEnabled;
                default:
                    return false;
            }
        }
    }
}