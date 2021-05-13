using System.IO;
using NAudio.Wave;
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
        public int GifCamWidth { get; set; } = 1280;
        public int GifCamHeight { get; set; } = 720;
        public int GifLength { get; set; } = 5000;
        public bool GifFps { get; set; } = false;
        public int CustomGifFps { get; set; } = 0;

        // Microphone
        public bool RecordingEnabled { get; set; } = false;
        public int MicIndex { get; set; } = 0;
        public int MicTimer { get; set; } = 1000;
        public int MicLength { get; set; } = 5000;
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
            =>  "```ahk\n" +
                $"Webcam:            {(CamEnabled ? "✅" : "❌")}\n" +
                $"Default Cam:       {DefaultCam} ({Constants.Cameras[DefaultCam].Name})\n" +
                $"Webcam Gif Length: {GifLength / 1000} second(s)\n" +
                $"Constant Gif FPS:  {(GifFps ? "✅" : "❌")}\n" +
                $"Custom Gif FPS:    {(!GifFps && CustomGifFps > 0 ? CustomGifFps + " FPS" : "❌")}\n" +
                $"Recording:         {(RecordingEnabled ? "✅" : "❌")}\n" +
                $"Recording Mic:     {WaveIn.GetCapabilities(MicIndex).ProductName}\n" +
                $"Screenshot:        {(SsEnabled ? "✅" : "❌")}\n" +
                $"Screenshot Blur:   {BlurAmount}\n" +
                $"Play:              {(PlayEnabled ? "✅" : "❌")}\n" +
                $"TTS:               {(TtsEnabled ? "✅" : "❌")}\n" +
                $"Play/TTS Timeout:  {Timeout / 1000} second(s)\n" +
                $"Processes:         {(ProcessesEnabled ? "✅" : "❌")}\n" +
                $"Folder:            {FolderPath}\n" +
                "```";

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
                case "webcamgif":
                    return CamEnabled;
                case "microphone":
                case "mic":
                case "recording":
                    return RecordingEnabled;
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
                default:
                    return false;
            }
        }
    }
}