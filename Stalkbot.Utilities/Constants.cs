using System.Windows.Media;
using AForge.Video.DirectShow;

namespace StalkbotGUI.Stalkbot.Utilities
{
    /// <summary>
    /// Reusable brushes
    /// </summary>
    internal static class Constants
    {
        public static readonly SolidColorBrush EnabledBrush = new SolidColorBrush(Colors.DarkGreen);
        public static readonly SolidColorBrush DisabledBrush = new SolidColorBrush(Colors.DarkRed);
        public static readonly SolidColorBrush LogInfoBrush = new SolidColorBrush(Colors.Green);
        public static readonly SolidColorBrush LogWarningBrush = new SolidColorBrush(Colors.Yellow);
        public static readonly SolidColorBrush LogErrorBrush = new SolidColorBrush(Colors.Red);
        public static readonly SolidColorBrush DefaultBrush = new SolidColorBrush(Colors.White);
        public static readonly FilterInfoCollection Cameras = new FilterInfoCollection(FilterCategory.VideoInputDevice);
    }
}
