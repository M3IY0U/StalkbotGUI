using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace StalkbotGUI.Stalkbot.Utilities
{
    class RequireEnabled : CheckBaseAttribute
    {
        public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
            => Task.FromResult(Config.Instance.IsEnabled(ctx.Command.Name.ToLower()));
    }
}
