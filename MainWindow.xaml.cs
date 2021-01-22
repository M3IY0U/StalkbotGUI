using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using StalkbotGUI.Stalkbot.Discord;
using StalkbotGUI.Stalkbot.Utilities;
using StalkbotGUI.Stalkbot.Utilities.UI;
using Config = StalkbotGUI.Stalkbot.Utilities.Config;
using ProgressBar = StalkbotGUI.Stalkbot.Utilities.UI.ProgressBar;

namespace StalkbotGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private StalkbotClient _client;
        public MainWindow()
        {
            InitializeComponent();
            Logger.Init(ref LogText, ref LogView);
            Task.Delay(1000);
            CheckRequirements();
            _client = new StalkbotClient();
            if (Config.Instance.AutoStartDiscord)
                new Action(() => OnOffButton_Click(null, null))();
            
            InitButtons();
        }
        
        #region ButtonHandlers
        private async void OnOffButton_Click(object sender, RoutedEventArgs e)
        {
            await _client.StartStopDiscord();
            OnOffButton.Background = new SolidColorBrush(_client.IsRunning ? Colors.DarkGreen : Colors.DarkRed);
            OnOffButton.Content = _client.IsRunning ? "On" : "Off";
        }
        private void WebcamToggle_Click(object sender, RoutedEventArgs e)
        {
            Config.Instance.CamEnabled = !Config.Instance.CamEnabled;
            Logger.Log($"Webcam: {Config.Instance.CamEnabled}", LogLevel.Info);
            UiHelpers.UpdateButton("webcam", ref WebcamToggle);
        }
        
        private void ScreenshotToggle_Click(object sender, RoutedEventArgs e)
        {
            Config.Instance.SsEnabled = !Config.Instance.SsEnabled;
            Logger.Log($"Screenshot: {Config.Instance.SsEnabled}", LogLevel.Info);
            UiHelpers.UpdateButton("screenshot", ref ScreenshotToggle);
        }

        private void TtsToggle_Click(object sender, RoutedEventArgs e)
        {
            Config.Instance.TtsEnabled = !Config.Instance.TtsEnabled;
            Logger.Log($"Text to Speech: {Config.Instance.TtsEnabled}", LogLevel.Info);
            UiHelpers.UpdateButton("tts", ref TtsToggle);
        }

        private void PlayToggle_Click(object sender, RoutedEventArgs e)
        {
            Config.Instance.PlayEnabled = !Config.Instance.PlayEnabled;
            Logger.Log($"Playsound: {Config.Instance.PlayEnabled}", LogLevel.Info);
            UiHelpers.UpdateButton("play", ref PlayToggle);
        }

        private void ProcToggle_Click(object sender, RoutedEventArgs e)
        {
            Config.Instance.ProcessesEnabled = !Config.Instance.ProcessesEnabled;
            Logger.Log($"Processes: {Config.Instance.ProcessesEnabled}", LogLevel.Info);
            UiHelpers.UpdateButton("proc", ref ProcToggle);
        }

        private void ClipboardToggle_Click(object sender, RoutedEventArgs e)
        {
            Config.Instance.ClipboardEnabled = !Config.Instance.ClipboardEnabled;
            Logger.Log($"Clipboard: {Config.Instance.ClipboardEnabled}", LogLevel.Info);
            UiHelpers.UpdateButton("clipboard", ref ClipboardToggle);
        }

        private async void UndoButton_Click(object sender, RoutedEventArgs e)
            => await _client.DeleteLastMessage();


        #endregion

        #region Utilities
        private void InitButtons()
        {
            UiHelpers.UpdateButton("webcam", ref WebcamToggle);
            UiHelpers.UpdateButton("screenshot", ref ScreenshotToggle);
            UiHelpers.UpdateButton("play", ref PlayToggle);
            UiHelpers.UpdateButton("tts", ref TtsToggle);
            UiHelpers.UpdateButton("proc", ref ProcToggle);
            UiHelpers.UpdateButton("clipboard", ref ClipboardToggle);
        }

        private async void CheckRequirements()
        {
            // Check for config.json
            if (!File.Exists("config.json"))
            {
                Logger.Log("config.json was not found, creating a default one", LogLevel.Warning);
                Config.Instance.SaveConfig();
            }

            if (File.Exists("ffmpeg.exe")) return;
            Logger.Log("ffmpeg was not found in directory, downloading it...", LogLevel.Warning);
            if (!await DownloadFfmpeg())
                Logger.Log("Error downloading ffmpeg, please try starting again or download it manually", LogLevel.Error);
            else
                Logger.Log("Successfully downloaded ffmpeg!", LogLevel.Info);
        }

        private async Task<bool> DownloadFfmpeg()
        {
            var error = false;
            await Dispatcher.Invoke(async () =>
            {
                var pbw = new ProgressBar();
                pbw.Show();

                using (var client = new WebClient())
                {
                    client.DownloadProgressChanged += (sender, args) => pbw.UpdateProgress(args.ProgressPercentage);
                    client.DownloadDataCompleted += (sender, args) => error = args.Cancelled || args.Error != null;
                    await client.DownloadFileTaskAsync(new Uri("https://timostestdoma.in/res/ffmpeg.exe"), "ffmpeg.exe");
                }
            });
            return !error;
        }
        #endregion
    }
}
