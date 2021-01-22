using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace StalkbotGUI.Stalkbot.Utilities
{
    public static class Logger
    {
        private static TextBlock logText = null;
        public static void Log(string message, LogLevel level)
        {
            if (logText == null) throw new Exception("Logger not initialized!");
            logText.Foreground = Color(level);
            logText.Text += $"{message}\n";
        }

        public static void BindToBlock(ref TextBlock textBlock)
            => logText = textBlock;

        private static SolidColorBrush Color(LogLevel level)
        {
            Color color;
            switch (level)
            {
                case LogLevel.Info:
                    color = Colors.Green;
                    break;
                case LogLevel.Error:
                    color = Colors.Red;
                    break;
                case LogLevel.Warning:
                    color = Colors.Yellow;
                    break;
                default:
                    color = Colors.White;
                    break;
            }

            return new SolidColorBrush(color);
        }
    }

    public enum LogLevel : ushort
    {
        Info = 0,
        Error = 1,
        Warning = 2
    }
}
