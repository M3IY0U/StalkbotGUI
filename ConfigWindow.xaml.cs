using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AForge.Video.DirectShow;
using Microsoft.WindowsAPICodePack.Dialogs;
using NAudio.Wave;
using StalkbotGUI.Stalkbot.Discord.Commands;
using StalkbotGUI.Stalkbot.Utilities;

namespace StalkbotGUI
{
    /// <summary>
    /// Interaction logic for ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow
    {
        /// <summary>
        /// Keeps track of the currently selected webcam
        /// </summary>
        private VideoCaptureDevice _selectedCam;

        /// <summary>
        /// Constructor
        /// </summary>
        public ConfigWindow()
        {
            InitializeComponent();
            FolderLabel.Content = string.IsNullOrEmpty(Config.Instance.FolderPath)
                ? "No folder selected."
                : Config.Instance.FolderPath;
            DurationInput.Text = $"{Config.Instance.Timeout}";
            BlurInput.Text = $"{Config.Instance.BlurAmount}";
            CamDelayInput.Text = $"{Config.Instance.CamTimer}";
            PrefixInput.Text = Config.Instance.Prefix;
            TokenInput.Text = Config.Instance.Token;
            AutoStartCheckBox.IsChecked = Config.Instance.AutoStartDiscord;
            MinimizeCheckBox.IsChecked = Config.Instance.MinimizeToTray;
            GifLengthInput.Text = $"{Config.Instance.GifLength}";
            var webcams = new List<string>();
            for (var i = 0; i < Constants.Cameras.Count; i++)
                webcams.Add(Constants.Cameras[i].Name);
            CamSelector.ItemsSource = webcams;
            CamSelector.SelectedIndex = Config.Instance.DefaultCam;
            _selectedCam = new VideoCaptureDevice(Constants.Cameras[Config.Instance.DefaultCam].MonikerString);
            GifFps.IsChecked = Config.Instance.GifFps;
            UpdateResBox(true);
            UpdateResBox(false);
            var mics = new List<string>();
            for (var i = 0; i < WaveIn.DeviceCount; i++)
                mics.Add(WaveIn.GetCapabilities(i).ProductName);
            MicSelector.ItemsSource = mics;
            MicSelector.SelectedIndex = Config.Instance.MicIndex;
            MicDelayTxtBx.Text = Config.Instance.MicTimer.ToString();
            MicLengthTxtBx.Text = Config.Instance.MicLength.ToString();
        }

        /// <summary>
        /// Handles clicking the folder picker
        /// </summary>
        /// <param name="sender">Button object</param>
        /// <param name="e">Event args</param>
        private void FolderSelect_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new CommonOpenFileDialog("Folder Resources") {IsFolderPicker = true})
            {
                if (dialog.ShowDialog() != CommonFileDialogResult.Ok) return;
                Config.Instance.FolderPath = dialog.FileName;
                FolderLabel.Content = $"{Config.Instance.FolderPath}";
            }
        }

        /// <summary>
        /// Handles clicking the folder reset button
        /// </summary>
        /// <param name="sender">Button object</param>
        /// <param name="e">Event args</param>
        private void FolderClear_Click(object sender, RoutedEventArgs e)
        {
            Config.Instance.FolderPath = "";
            FolderLabel.Content = "No folder selected.";
        }

        /// <summary>
        /// Handles changing the camera selection
        /// </summary>
        /// <param name="sender">Combobox object</param>
        /// <param name="e">Event args</param>
        private void CamSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Config.Instance.DefaultCam = CamSelector.SelectedIndex;
            _selectedCam = new VideoCaptureDevice(Constants.Cameras[Config.Instance.DefaultCam].MonikerString);
            UpdateResBox(true);
            UpdateResBox(false);
        }

        /// <summary>
        /// Handles changing the microphone selection
        /// </summary>
        /// <param name="sender">Combobox object</param>
        /// <param name="e">Event args</param>
        private void MicSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Config.Instance.MicIndex = MicSelector.SelectedIndex;
            WaveIn.GetCapabilities(Config.Instance.MicIndex);
        }

        /// <summary>
        /// Updates the content of the corresponding resolution combobox
        /// </summary>
        /// <param name="isGif">Whether the gif or normal resolution should be updated</param>
        private void UpdateResBox(bool isGif)
        {
            var cb = isGif ? GifResolutionSelector : ResolutionSelector;
            var items =
                _selectedCam.VideoCapabilities.Select(x => $"{x.FrameSize.Width}x{x.FrameSize.Height}").ToList();
            cb.ItemsSource = items;
            cb.SelectedIndex =
                items.FindIndex(s
                    => s.Contains(isGif
                           ? Config.Instance.GifCamWidth.ToString()
                           : Config.Instance.CamWidth.ToString())
                       && s.Contains(isGif
                           ? Config.Instance.GifCamHeight.ToString()
                           : Config.Instance.CamHeight.ToString()));
        }

        /// <summary>
        /// Handles changing the resolution selection
        /// </summary>
        /// <param name="sender">Combobox object</param>
        /// <param name="e">Event args</param>
        private void ResolutionSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Config.Instance.CamWidth = Convert.ToInt32(((string)ResolutionSelector.SelectedItem).Split('x').First());
                Config.Instance.CamHeight = Convert.ToInt32(((string)ResolutionSelector.SelectedItem).Split('x').Last());
            }
            catch (Exception)
            {
                Logger.Log("Unable to determine camera resolution of the selected camera", LogLevel.Warning);
            }
        }

        /// <summary>
        /// Handles the window being closed via the X button
        /// </summary>
        /// <param name="sender">Window object</param>
        /// <param name="e">Event args</param>
        private void Window_Closed(object sender, EventArgs e)
        {
            Config.Instance.SaveConfig();
            Logger.Log("Config saved", LogLevel.Info);
        }

        /// <summary>
        /// Handles text changing in the cam delay input
        /// </summary>
        /// <param name="sender">Textbox object</param>
        /// <param name="e">Event args</param>
        private void CamDelayInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(CamDelayInput.Text))
                Config.Instance.CamTimer = Convert.ToInt32(CamDelayInput.Text);
        }

        /// <summary>
        /// Handles text changing in the blur input
        /// </summary>
        /// <param name="sender">Textbox object</param>
        /// <param name="e">Event args</param>
        private void BlurInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(BlurInput.Text))
                Config.Instance.BlurAmount = Convert.ToDouble(BlurInput.Text);
        }

        /// <summary>
        /// Handles text changing in the duration/timeout input
        /// </summary>
        /// <param name="sender">Textbox object</param>
        /// <param name="e">Event args</param>
        private void DurationInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(DurationInput.Text))
                Config.Instance.Timeout = Convert.ToDouble(DurationInput.Text);
        }

        /// <summary>
        /// Handles checkbox changes for minimizing
        /// </summary>
        /// <param name="sender">Checkbox object</param>
        /// <param name="e">Event args</param>
        private void MinimizeCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (MinimizeCheckBox.IsChecked.HasValue)
                Config.Instance.MinimizeToTray = MinimizeCheckBox.IsChecked.Value;
        }

        /// <summary>
        /// Handles checkbox changes for auto starting
        /// </summary>
        /// <param name="sender">Checkbox object</param>
        /// <param name="e">Event args</param>
        private void AutoStartCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (AutoStartCheckBox.IsChecked.HasValue)
                Config.Instance.AutoStartDiscord = AutoStartCheckBox.IsChecked.Value;
        }

        /// <summary>
        /// Handles text changing in the token input
        /// </summary>
        /// <param name="sender">Textbox object</param>
        /// <param name="e">Event args</param>
        private void TokenInput_TextChanged(object sender, TextChangedEventArgs e)
            => Config.Instance.Token = TokenInput.Text;

        /// <summary>
        /// Handles text changing in the prefix input
        /// </summary>
        /// <param name="sender">Textbox input</param>
        /// <param name="e">Event args</param>
        private void PrefixInput_TextChanged(object sender, TextChangedEventArgs e)
            => Config.Instance.Prefix = PrefixInput.Text;

        /// <summary>
        /// Handles text changing in the gif length input
        /// </summary>
        /// <param name="sender">Textbox object</param>
        /// <param name="e">Event args</param>
        private void GifLengthInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(GifLengthInput.Text))
                Config.Instance.GifLength = int.Parse(GifLengthInput.Text);
        }

        /// <summary>
        /// Handles checkbox changes for gif fps
        /// </summary>
        /// <param name="sender">Checkbox object</param>
        /// <param name="e">Event args</param>
        private void GifFps_Click(object sender, RoutedEventArgs e)
        {
            if (GifFps.IsChecked.HasValue)
                Config.Instance.GifFps = GifFps.IsChecked.Value;
        }

        /// <summary>
        /// Handles changing the gif resolution selection
        /// </summary>
        /// <param name="sender">Combobox object</param>
        /// <param name="e">Event args</param>
        private void GifResolutionSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Config.Instance.GifCamWidth = Convert.ToInt32(((string)GifResolutionSelector.SelectedItem).Split('x').First());
                Config.Instance.GifCamHeight = Convert.ToInt32(((string)GifResolutionSelector.SelectedItem).Split('x').Last());
            }
            catch (Exception) { /* ignored */ }
        }

        private void MicDelayTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(MicDelayTxtBx.Text))
                Config.Instance.MicTimer = Convert.ToInt32(MicDelayTxtBx.Text);
        }

        private async void TestScreenshotButton_Click(object sender, RoutedEventArgs e)
            => await Screenshot.TestScreenshotAsync();

        private void MicLengthTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(MicLengthTxtBx.Text))
                Config.Instance.MicLength = Convert.ToInt32(MicLengthTxtBx.Text);
        }
    }
}
