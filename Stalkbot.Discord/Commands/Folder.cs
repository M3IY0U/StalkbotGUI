using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using StalkbotGUI.Stalkbot.Utilities;

namespace StalkbotGUI.Stalkbot.Discord.Commands
{
    public class Folder : AlertCommand
    {
        /// <summary>
        /// Command for responding with a random file from the users set folder
        /// </summary>
        /// <param name="ctx">Context this command has been executed in</param>
        /// <param name="search">Optional search term for finding specific files</param>
        /// <returns>The built task</returns>
        [RequireEnabled, Command("folder"), Aliases("f"),
         Description("Sends a random (or specific file) from a predefined folder.")]
        public async Task FolderTask(CommandContext ctx, [Description("Term to search for, leave blank for random file.")][RemainingText] string search = "")
        {
            var rng = new Random();
            var files = CommandHelper.IndexFiles();

            var file = files[rng.Next(files.Length)];
            if (!string.IsNullOrEmpty(search))
            {
                files = files.Where(x => x.ToLower().Contains(search.ToLower())).ToArray();
                file = files[rng.Next(files.Length)];
            }

            var fileName = file.Substring(file.LastIndexOf(Path.DirectorySeparatorChar) + 1);

            if (new FileInfo(file).Length > 8_388_608)
            {
                await ctx.RespondAsync($"File `{fileName}` was over 8MB");
                throw new Exception($"File '{fileName}' was over Discord's size limit");
            }

            Logger.Log($"Folder requested by {ctx.User.Username} in #{ctx.Channel.Name} ({ctx.Guild.Name})" +
                       $"\n\t=> Sending file \"{fileName}\"",
            LogLevel.Info);
            
            var msg = new DiscordMessageBuilder()
                .WithReply(ctx.Message.Id)
                .WithFile(new FileStream(file, FileMode.Open));

            StalkbotClient.UpdateLastMessage(await ctx.RespondAsync(msg));
        }
    }
}
