using System;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;

namespace StalkbotGUI.Stalkbot.Utilities
{
    /// <summary>
    /// Logger
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Contains the actual log texts
        /// </summary>
        private static TextBlock _logText;

        /// <summary>
        /// Log "container"
        /// </summary>
        private static ScrollViewer _viewer;

        /// <summary>
        /// Logs a message to the window
        /// </summary>
        /// <param name="message">The message to be logged</param>
        /// <param name="level">The severity of the message</param>
        public static void Log(string message, LogLevel level)
        {
            if (_logText == null) throw new Exception("Logger not initialized!");

            _logText.Dispatcher.Invoke(() => _logText.Inlines.Add(
                new Run($"[{DateTime.Now.ToLongTimeString()}] {message}{Environment.NewLine}") { Foreground = Color(level) }));
            _viewer.Dispatcher.Invoke(() => _viewer.ScrollToEnd());
        }

        /// <summary>
        /// Binds the logger to the window controls
        /// </summary>
        /// <param name="textBlock">The log texts</param>
        /// <param name="scrollViewer">The log "container"</param>
        public static void Init(ref TextBlock textBlock, ref ScrollViewer scrollViewer)
        {
            _logText = textBlock;
            _viewer = scrollViewer;
        }

        /// <summary>
        /// Level to color converter
        /// </summary>
        /// <param name="level">Log severity</param>
        /// <returns>A brush with a color indicating the severity</returns>
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

    /// <summary>
    /// Severity levels
    /// </summary>
    public enum LogLevel : ushort
    {
        Info = 0,
        Error = 1,
        Warning = 2
    }
}
