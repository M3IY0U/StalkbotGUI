using System;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace StalkbotGUI.Stalkbot.Utilities
{
    public static class Logger
    {
        private static TextBlock _logText;
        private static ScrollViewer _viewer;
        public static void Log(string message, LogLevel level)
        {
            if (_logText == null) throw new Exception("Logger not initialized!");
            
            _logText.Inlines.Add(new Run($"{message}{Environment.NewLine}") { Foreground = Color(level)});
            _viewer.ScrollToEnd();
        }

        public static void Init(ref TextBlock textBlock, ref ScrollViewer scrollViewer)
        {
            _logText = textBlock;
            _viewer = scrollViewer;
        }

        private static SolidColorBrush Color(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Info:
                    return Constants.LogInfoBrush;
                case LogLevel.Error:
                    return Constants.LogErrorBrush;
                case LogLevel.Warning:
                    return Constants.LogWarningBrush;
                default:
                    return Constants.DefaultBrush;
            }
        }
    }

    public enum LogLevel : ushort
    {
        Info = 0,
        Error = 1,
        Warning = 2
    }
}
