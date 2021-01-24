using System.IO;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace StalkbotGUI.Stalkbot.Utilities
{
    /// <summary>
    /// Class containing helper methods for commands
    /// </summary>
    public class CommandHelper
    {
        /// <summary>
        /// Indexes the files if there was a path provided
        /// </summary>
        /// <returns>An array of indexed files</returns>
        public static string[] IndexFiles()
            => !string.IsNullOrEmpty(Config.Instance.FolderPath) ? SearchFiles(Config.Instance.FolderPath) : new[] { "" };

        /// <summary>
        /// Recursive function that crawls through subdirectories
        /// </summary>
        /// <param name="path">Root directory to index</param>
        /// <returns>File array of the files in the current directory</returns>
        private static string[] SearchFiles(string path)
        {
            var files = Directory.GetFiles(path);
            var dirs = Directory.GetDirectories(path);
            return dirs.Length == 0
                ? files
                : dirs.Aggregate(files, (current, dir) => current.Union(SearchFiles(dir)).ToArray());
        }

        /// <summary>
        /// Plays an alert a recognized command has been triggered 
        /// </summary>
        /// <param name="sender">The discord client that fired the command</param>
        /// <param name="e">Event args</param>
        /// <returns>The built task</returns>
        public static Task PlayAlert(DiscordClient sender, MessageCreateEventArgs e)
        {
            // check if message was a command
            if (!e.Message.Content.StartsWith(Config.Instance.Prefix)) return Task.CompletedTask;
            // check there is a command like that and out it
            if (!sender.GetCommandsNext().RegisteredCommands
                .TryGetValue(e.Message.Content.Substring(Config.Instance.Prefix.Length).Split(' ').First(),
                    out var cmd)) return Task.CompletedTask;
            // if the command is disabled or there's no file, abort
            if (!Config.Instance.IsEnabled(cmd.Name.ToLower()) || !File.Exists($"{cmd.Name.ToLower()}.wav")) return Task.CompletedTask;

            // play the audio
            using (var player = new SoundPlayer($"{cmd.Name.ToLower()}.wav"))
                player.Play();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Handles a command error
        /// </summary>
        /// <param name="sender">CommandsNext extension object</param>
        /// <param name="e">Event args</param>
        /// <returns>The built task</returns>
        public static async Task CommandErrored(CommandsNextExtension sender, CommandErrorEventArgs e)
        {
            // "ignore" non-existent commands
            if (e.Exception.Message.Contains("command was not found"))
            {
                await e.Context.Message.CreateReactionAsync(DiscordEmoji.FromUnicode("❓"));
                return;
            }

            // check if the command was just disabled
            if (((ChecksFailedException)e.Exception).FailedChecks.OfType<RequireEnabled>().Any())
            {
                Logger.Log($"{e.Context.User.Username} used {e.Command.Name} command in #{e.Context.Channel.Name}, but it was disabled.", LogLevel.Warning);
                await e.Context.Message.CreateReactionAsync(DiscordEmoji.FromUnicode("🔕"));
                return;
            }

            // log an actual error
            await e.Context.Message.CreateReactionAsync(DiscordEmoji.FromUnicode("❌"));
            Logger.Log($"Exception in command {e.Command.Name}! Message: {e.Exception.Message}", LogLevel.Error);
        }
    }
}