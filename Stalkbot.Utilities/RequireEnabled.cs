using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace StalkbotGUI.Stalkbot.Utilities
{
    [AttributeUsage(AttributeTargets.Method)]
    class RequireEnabled : CheckBaseAttribute
    {
        public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
            => help 
                ? Task.FromResult(true) 
                : Task.FromResult(Config.Instance.IsEnabled(ctx.Command.Name.ToLower()));
    }
}
