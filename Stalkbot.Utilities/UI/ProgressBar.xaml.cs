using System.Windows;

namespace StalkbotGUI.Stalkbot.Utilities.UI
{
    /// <summary>
    /// Interaction logic for ProgressBar.xaml
    /// </summary>
    public partial class ProgressBar : Window
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ProgressBar(string filename)
        {
            InitializeComponent();
            Title = $"Downloading {filename}...";
        }

        /// <summary>
        /// Updates the progressbar
        /// </summary>
        /// <param name="percentage">New value</param>
        public void UpdateProgress(int percentage)
        {
            // When progress is reported, update the progress bar control.
            PbLoad.Value = percentage;

            // When progress reaches 100%, close the progress bar window.
            if (percentage == 100)
            {
                Close();
            }
        }
    }
}
