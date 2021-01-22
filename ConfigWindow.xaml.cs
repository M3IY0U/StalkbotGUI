using System;
using System.Collections.Generic;
using System.Windows;
using AForge.Video.DirectShow;
using Microsoft.WindowsAPICodePack.Dialogs;
using StalkbotGUI.Stalkbot.Utilities;

namespace StalkbotGUI
{
    /// <summary>
    /// Interaction logic for ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ConfigWindow()
        {
            InitializeComponent();
            FolderLabel.Content = Config.Instance.FolderPath;
            DurationInput.Text = $"{Config.Instance.Timeout}";
            BlurInput.Text = $"{Config.Instance.BlurAmount}";
            WidthInput.Text = $"{Config.Instance.CamWidth}";
            HeightInput.Text = $"{Config.Instance.CamHeight}";
            CamDelayInput.Text = $"{Config.Instance.CamTimer}";
            AutoStartCheckBox.IsChecked = Config.Instance.AutoStartDiscord;
            MinimizeCheckBox.IsChecked= Config.Instance.MinimizeToTray;
            var devices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            var webcams = new List<string>();
            for (var i = devices.Count - 1; i >= 0; i--)
                webcams.Add(devices[i].Name);
            CamSelector.ItemsSource = webcams;
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
                FolderLabel.Content = $"Current Folder:\n{Config.Instance.FolderPath}";
            }
        }

        /// <summary>
        /// Handles changing the camera selection
        /// </summary>
        /// <param name="sender">Combobox object</param>
        /// <param name="e">Event args</param>
        private void CamSelector_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
            => Config.Instance.DefaultCam = CamSelector.SelectedIndex;

        
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
        private void CamDelayInput_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(CamDelayInput.Text))
                Config.Instance.CamTimer = Convert.ToInt32(CamDelayInput.Text);
        }

        /// <summary>
        /// Handles text changing in the height input
        /// </summary>
        /// <param name="sender">Textbox object</param>
        /// <param name="e">Event args</param>
        private void HeightInput_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(HeightInput.Text))
                Config.Instance.CamHeight = Convert.ToInt32(HeightInput.Text);
        }

        /// <summary>
        /// Handles text changing in the width input
        /// </summary>
        /// <param name="sender">Textbox object</param>
        /// <param name="e">Event args</param>
        private void WidthInput_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(WidthInput.Text))
                Config.Instance.CamWidth = Convert.ToInt32(WidthInput.Text);
        }

        /// <summary>
        /// Handles text changing in the blur input
        /// </summary>
        /// <param name="sender">Textbox object</param>
        /// <param name="e">Event args</param>
        private void BlurInput_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(BlurInput.Text))
                Config.Instance.BlurAmount = Convert.ToDouble(BlurInput.Text);
        }

        /// <summary>
        /// Handles text changing in the duration/timeout input
        /// </summary>
        /// <param name="sender">Textbox object</param>
        /// <param name="e">Event args</param>
        private void DurationInput_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if(!string.IsNullOrEmpty(DurationInput.Text))
                Config.Instance.Timeout = Convert.ToDouble(DurationInput.Text);
        }

        private void MinimizeCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if(MinimizeCheckBox.IsChecked.HasValue)
                Config.Instance.MinimizeToTray = MinimizeCheckBox.IsChecked.Value;
        }

        private void AutoStartCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (AutoStartCheckBox.IsChecked.HasValue)
                Config.Instance.AutoStartDiscord = AutoStartCheckBox.IsChecked.Value;
        }
    }
}