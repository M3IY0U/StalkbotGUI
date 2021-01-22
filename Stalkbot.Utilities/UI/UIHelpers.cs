using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace StalkbotGUI.Stalkbot.Utilities.UI
{
    public class UiHelpers
    {
        public static void UpdateButton(string which, ref Button button)
        {
            var color = Config.Instance.IsEnabled(which) ? Colors.DarkGreen :Colors.DarkRed;
            button.Background = new SolidColorBrush(color);
        }
    }
}
