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
        public string[] Files { get; }
        public Random Rng;

        public Folder(string[] files, Random rng)
            => (Files, Rng) = (files, rng);

        /// <summary>
        /// Responds with a random file from the users set folder
        /// </summary>
        /// <param name="ctx">Context this command has been executed in</param>
        /// <param name="search">Optional search term for finding specific files</param>
        /// <returns>The built task</returns>
        [RequireEnabled, Command("folder"), Aliases("f"), Description("Sends a random (or specific file) from a predefined folder.")]
        public async Task SendRandomFile(CommandContext ctx, [Description("Term to search for, leave blank for random file.")] string search = "")
        {
            var file = Files[Rng.Next(Files.Length)];
            if (!string.IsNullOrEmpty(search))
            {
                foreach (var ef in Files)
                {
                    if (!ef.ToLower().Contains(search.ToLower())) continue;
                    file = ef;
                    break;
                }
            }

            StalkbotClient.UpdateLastMessage(await ctx.RespondWithFileAsync(file, file.Substring(file.LastIndexOf(Path.DirectorySeparatorChar) + 1)));
        }
    }
}
