using System.Windows.Controls;

namespace StalkbotGUI.Stalkbot.Utilities.UI
{
    /// <summary>
    /// Class containing UI utility functions
    /// </summary>
    public class UiHelpers
    {
        /// <summary>
        /// Updates the specified button to the correct color
        /// </summary>
        /// <param name="which">Which command to update</param>
        /// <param name="button">The button reference</param>
        public static void UpdateButton(string which, ref Button button)
            => button.Background = Config.Instance.IsEnabled(which) ? Constants.EnabledBrush : Constants.DisabledBrush;
    }
}
