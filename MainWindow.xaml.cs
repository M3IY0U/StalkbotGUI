using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using StalkbotGUI.Stalkbot.Discord;
using StalkbotGUI.Stalkbot.Utilities;
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
            Logger.BindToBlock(ref LogText);
            CheckRequirements();
            _client= new StalkbotClient();
            
        }

        private void CheckRequirements()
        {
            // Check for config.json
            if (!File.Exists("config.json"))
            {
                Logger.Log("config.json was not found, creating a default one", LogLevel.Warning);
                Config.Instance.SaveConfig();
            }

            if (File.Exists("ffmpeg.exe")) return;
            Logger.Log("ffmpeg was not found in directory, downloading it...", LogLevel.Warning);
            if (!DownloadFfmpeg())
                Logger.Log("Error downloading ffmpeg, please try starting again or download it manually", LogLevel.Error);
            else
                Logger.Log("Successfully downloaded ffmpeg!", LogLevel.Info);
        }

        private bool DownloadFfmpeg()
        {
            var error = false;
            Dispatcher.Invoke(() =>
            {
                var pbw = new ProgressBar();
                pbw.Show();

                using (var client = new WebClient())
                {
                    client.DownloadProgressChanged += (sender, args) => pbw.UpdateProgress(args.ProgressPercentage);
                    client.DownloadDataCompleted += (sender, args) => error = args.Cancelled || args.Error != null;
                    client.DownloadFileAsync(new Uri("https://timostestdoma.in/res/ffmpeg.exe"), "ffmpeg.exe");
                }
            });
            return !error;
        }

        private void WebcamToggle_Click(object sender, RoutedEventArgs e)
        {
            Logger.Log("Webcam clicked", LogLevel.Info);
        }

        private async void OnOffButton_Click(object sender, RoutedEventArgs e)
        {
            if (OnOffButton.Content.ToString() == "On")
            {
                OnOffButton.Background = new SolidColorBrush(Colors.DarkRed);
                OnOffButton.Content = "Off";
            }
            else
            {
                OnOffButton.Background = new SolidColorBrush(Colors.DarkGreen);
                OnOffButton.Content = "On";
            }
            await _client.StartStopDiscord();
        }
    }
}
