using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace StalkbotGUI.Stalkbot.Utilities
{
    internal static class Constants
    {
        public static readonly SolidColorBrush EnabledBrush = new SolidColorBrush(Colors.DarkGreen);
        public static readonly SolidColorBrush DisabledBrush = new SolidColorBrush(Colors.DarkRed);
        public static readonly SolidColorBrush LogInfoBrush = new SolidColorBrush(Colors.Green);
        public static readonly SolidColorBrush LogWarningBrush = new SolidColorBrush(Colors.Yellow);
        public static readonly SolidColorBrush LogErrorBrush = new SolidColorBrush(Colors.Red);
        public static readonly SolidColorBrush DefaultBrush = new SolidColorBrush(Colors.White);
    }
}
