using System.Collections.Generic;
using System.Windows;
using AForge.Video.DirectShow;
using Microsoft.WindowsAPICodePack.Dialogs;
using StalkbotGUI.Stalkbot.Utilities;
using System.Linq;

namespace StalkbotGUI
{
    /// <summary>
    /// Interaction logic for ConfigWindow.xaml
    /// </summary>
    public partial class ConfigWindow : Window
    {
        public ConfigWindow()
        {
            InitializeComponent();
            FolderLabel.Content = Config.Instance.FolderPath;
            DurationInput.Text = $"{Config.Instance.Timeout}";
            BlurInput.Text = $"{Config.Instance.BlurAmount}";
            WidthInput.Text = $"{Config.Instance.CamWidth}";
            HeightInput.Text = $"{Config.Instance.CamHeight}";
            CamDelayInput.Text = $"{Config.Instance.CamTimer}";
            var devices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            var webcams = new List<string>();
            for (var i = devices.Count - 1; i >= 0; i--)
                webcams.Add(devices[i].Name);
            CamSelector.ItemsSource = webcams;
        }

        private void FolderSelect_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new CommonOpenFileDialog("Folder Resources") {IsFolderPicker = true})
            {
                if (dialog.ShowDialog() != CommonFileDialogResult.Ok) return;
                Config.Instance.FolderPath = dialog.FileName;
                FolderLabel.Content = $"Current Folder:\n{Config.Instance.FolderPath}";
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Config.Instance.SaveConfig();
            Logger.Log("Config saved", LogLevel.Info);
        }

        private void CamSelector_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
            => Config.Instance.DefaultCam = CamSelector.SelectedIndex;
    }
}
