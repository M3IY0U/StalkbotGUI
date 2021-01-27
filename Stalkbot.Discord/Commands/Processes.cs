using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using StalkbotGUI.Stalkbot.Utilities;

namespace StalkbotGUI.Stalkbot.Discord.Commands
{
    public class Processes : AlertCommand
    {
        /// <summary>
        /// Command for listing top 15 most ram intensive processes
        /// </summary>
        /// <param name="ctx">Context this command has been executed in</param>
        /// <returns>The built task</returns>
        [RequireEnabled, Command("processes"), Aliases("proc"),
        Description("Returns the top 15 ram heavy processes in a table.")]
        public async Task ProcessesTask(CommandContext ctx)
        {
            Logger.Log($"Processes requested by {ctx.User.Username} in #{ctx.Channel.Name} ({ctx.Guild.Name})",
                LogLevel.Info);
            var table = new ConsoleTable("Name", "RAM", "Uptime");
            Process.GetProcesses()
                .Where(x => x.SessionId == Process.GetCurrentProcess().SessionId)
                .OrderByDescending(p => p.PrivateMemorySize64)
                .Take(15)
                .ToList()
                .ForEach(process =>
                {
                    try
                    {
                        table.AddRow($"{process.ProcessName}", $"{process.PrivateMemorySize64 / 1_000_000}MB",
                            $"{DateTime.Now - process.StartTime:h'h 'm'm 's's'}");
                    }
                    catch
                    {
                        table.AddRow($"{process.ProcessName}", $"{process.PrivateMemorySize64 / 1_000_000}MB",
                            "Not available");
                    }
                });
            
            var tableString = table.Configure(x => x.NumberAlignment = Alignment.Right).ToMinimalString();
            StalkbotClient.UpdateLastMessage(await ctx.RespondAsync($"```{tableString}```"));
        }
    }
}
