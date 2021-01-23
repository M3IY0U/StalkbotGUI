using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly DiscordClient _client;
        
        /// <summary>
        /// Command handler
        /// </summary>
        private CommandsNextExtension _commandsNext;

        /// <summary>
        /// Holds the last message if the user wants it deleted
        /// </summary>
        private DiscordMessage _lastResponse;

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

            // dependency injection (might be overkill but idc)
            var dependencies = new ServiceCollection();
            dependencies.AddSingleton(CommandHelper.IndexFiles());

            // command config
            _commandsNext = _client.UseCommandsNext(new CommandsNextConfiguration
            {
                EnableDms = false,
                StringPrefixes = new List<string> { Config.Instance.Prefix },
                Services = dependencies.BuildServiceProvider()
            });

            // register commands + hook events
            _commandsNext.RegisterCommands(Assembly.GetEntryAssembly());
            _client.MessageCreated += CommandHelper.PlayAlert;
        }

        /// <summary>
        /// Deletes the last message the bot sent
        /// </summary>
        /// <returns>The built task</returns>
        public async Task DeleteLastMessage()
        {
            if (_lastResponse == null)
                return;
            await _lastResponse.DeleteAsync();
            _lastResponse = null;
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