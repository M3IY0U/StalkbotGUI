using System.Windows.Controls;

namespace StalkbotGUI.Stalkbot.Utilities.UI
{
    public class UiHelpers
    {
        public static void UpdateButton(string which, ref Button button)
            => button.Background = Config.Instance.IsEnabled(which) ? Constants.EnabledBrush : Constants.DisabledBrush;
    }
}
