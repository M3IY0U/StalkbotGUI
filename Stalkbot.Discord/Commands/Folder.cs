using System;
using System.IO;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using StalkbotGUI.Stalkbot.Utilities;

namespace StalkbotGUI.Stalkbot.Discord.Commands
{
    public class Folder : BaseCommandModule
    {
        /// <summary>
        /// Command for responding with a random file from the users set folder
        /// </summary>
        /// <param name="ctx">Context this command has been executed in</param>
        /// <param name="search">Optional search term for finding specific files</param>
        /// <returns>The built task</returns>
        [RequireEnabled, Command("folder"), Aliases("f"),
         Description("Sends a random (or specific file) from a predefined folder.")]
        public async Task FolderTask(CommandContext ctx, [Description("Term to search for, leave blank for random file.")] string search = "")
        {
            var rng = new Random();
            var files = CommandHelper.IndexFiles();

            var file = files[rng.Next(files.Length)];
            if (!string.IsNullOrEmpty(search))
            {
                foreach (var ef in files)
                {
                    if (!ef.ToLower().Contains(search.ToLower())) continue;
                    file = ef;
                    break;
                }
            }

            var fileName = file.Substring(file.LastIndexOf(Path.DirectorySeparatorChar) + 1);
            Logger.Log($"Folder requested by {ctx.User.Username} in #{ctx.Channel.Name} ({ctx.Guild.Name})" +
                       $"\n\t=> Sending file \"{fileName}\"",
                LogLevel.Info);

            StalkbotClient.UpdateLastMessage(await ctx.RespondWithFileAsync(file, fileName));
        }
    }
}
