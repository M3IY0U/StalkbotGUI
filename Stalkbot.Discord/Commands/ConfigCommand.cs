using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using StalkbotGUI.Stalkbot.Utilities;

namespace StalkbotGUI.Stalkbot.Discord.Commands
{
    public class ConfigCommand : BaseCommandModule
    {
        [Command("config"), Aliases("cfg")]
        public async Task PrintConfig(CommandContext ctx)
            => await ctx.RespondAsync(Config.Instance.ToString());
    }
}
