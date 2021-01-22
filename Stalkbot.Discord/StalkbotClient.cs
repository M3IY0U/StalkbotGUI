using System.Collections.Generic;
using System.Reflection;
using DSharpPlus;
using DSharpPlus.CommandsNext;
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
    }
}