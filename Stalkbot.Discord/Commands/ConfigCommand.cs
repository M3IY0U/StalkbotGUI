using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using StalkbotGUI.Stalkbot.Utilities;

namespace StalkbotGUI.Stalkbot.Discord.Commands
{
    public class ConfigCommand : BaseCommandModule
    {
        /// <summary>
        /// Displays relevant config items to discord users
        /// </summary>
        /// <param name="ctx">Context this command was executed in</param>
        /// <returns>The built task</returns>
        [Command("config"), Aliases("cfg"),
        Description("Prints out all relevant stalk information to Discord.")]
        public async Task ConfigTask(CommandContext ctx)
            => await ctx.RespondAsync(Config.Instance.ToString());
    }
}
