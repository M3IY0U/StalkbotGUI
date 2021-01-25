using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using StalkbotGUI.Stalkbot.Utilities;

namespace StalkbotGUI.Stalkbot.Discord
{
    /// <summary>
    /// Client responsible for interacting with discord
    /// </summary>
    public class StalkbotClient
    {
        /// <summary>
        /// The actual discord client
        /// </summary>
        private DiscordClient _client;

        /// <summary>
        /// Command handler
        /// </summary>
        private CommandsNextExtension _commandsNext;

        /// <summary>
        /// Holds the last message if the user wants it deleted
        /// </summary>
        private static readonly Stack<DiscordMessage> LastResponses = new Stack<DiscordMessage>();

        /// <summary>
        /// Flag keeping track of the bots status
        /// </summary>
        public bool IsRunning { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public StalkbotClient()
        {
            // init actual bot client
            _client = new DiscordClient(new DiscordConfiguration
            {
                Token = Config.Instance.Token
            });
            
            // command config
            _commandsNext = _client.UseCommandsNext(new CommandsNextConfiguration
            {
                EnableDms = false,
                StringPrefixes = new List<string> { Config.Instance.Prefix }
            });

            // register commands + hook events
            _commandsNext.RegisterCommands(Assembly.GetEntryAssembly());
            _commandsNext.CommandErrored += CommandHelper.CommandErrored;
            _client.MessageCreated += CommandHelper.PlayAlert;
        }

        /// <summary>
        /// Refreshes the discord client
        /// </summary>
        public void ReloadDiscordClient()
            => _client = new DiscordClient(new DiscordConfiguration {Token = Config.Instance.Token});

        /// <summary>
        /// Deletes the last message the bot sent
        /// </summary>
        /// <returns>The built task</returns>
        public async Task Undo()
        {
            if (LastResponses.Count == 0)
            {
                Logger.Log("Nothing left to delete!", LogLevel.Error);
                return;
            }

            Logger.Log($"Deleted last response in #{LastResponses.Peek().Channel.Name} ({LastResponses.Peek().Channel.Guild.Name})", LogLevel.Warning);
            await LastResponses.Pop().DeleteAsync();
        }

        /// <summary>
        /// Starts or stops discord
        /// </summary>
        /// <returns>The built task</returns>
        public async Task StartStopDiscord()
        {
            try
            {
                if (_client.Ping == 0) //TODO: I'm not sure if ping is all that reliable but for now it works
                {
                    Logger.Log("Connecting to Discord...", LogLevel.Info);
                    await _client.ConnectAsync();
                    await Task.Delay(2000);
                    await _client.UpdateStatusAsync(new DiscordActivity(
                        $"{_client.CurrentApplication.Owners.First().Username}",
                        ActivityType.Watching));
                    Logger.Log($"Successfully connected to Discord on {_client.Guilds.Count} Servers:\n\t{string.Join("\n\t", _client.Guilds.Values)}", LogLevel.Info);
                    IsRunning = true;
                }
                else
                {
                    Logger.Log("Disconnecting from Discord...", LogLevel.Info);
                    await _client.DisconnectAsync();
                    IsRunning = false;
                    Logger.Log("Successfully disconnected", LogLevel.Info);
                }
            }
            catch (Exception e)
            {
                Logger.Log($"Error logging into Discord: {e.Message}", LogLevel.Error);
            }
        }

        /// <summary>
        /// Updates the bots last message
        /// </summary>
        /// <param name="msg">The new message</param>
        public static void UpdateLastMessage(DiscordMessage msg)
            => LastResponses.Push(msg);

        /// <summary>
        /// Disconnects and disposes of members
        /// </summary>
        public async void Dispose()
        {
            await _client.DisconnectAsync();
            _client.Dispose();
            _commandsNext = null;
        }
    }
}