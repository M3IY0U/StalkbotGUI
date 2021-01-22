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
    public class StalkbotClient
    {
        private readonly DiscordClient _client;
        private CommandsNextExtension _commandsNext;

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

        public async Task StartStopDiscord()
        {
            if (_client.Ping == 0)
            {
                await _client.ConnectAsync();
                await Task.Delay(2000);
                await _client.UpdateStatusAsync(new DiscordActivity(
                    $"{_client.CurrentApplication.Owners.First().Username}",
                    ActivityType.Watching));
            }
            else
            {
                await _client.DisconnectAsync();
            }
        }
    }
}