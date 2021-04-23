using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using StalkbotGUI.Stalkbot.Utilities;
using Image = SixLabors.ImageSharp.Image;

namespace StalkbotGUI.Stalkbot.Discord.Commands
{
    public class Screenshot : AlertCommand
    {
        private const string _liveFilename = "screenshot.png";
        private const string _testFilename = "test_ss.png";

        /// <summary>
        /// Captures a screenshot of all monitors
        /// </summary>
        /// <param name="ctx">Context this command has been executed in</param>
        /// <returns>The built task</returns>
        [RequireEnabled, Command("screenshot"), Aliases("ss"), 
         Cooldown(1, 5, CooldownBucketType.Global),
         Description("Captures a screenshot.")]
        public async Task ScreenshotTask(CommandContext ctx)
        {
            Logger.Log($"Screenshot requested by {ctx.User.Username} in #{ctx.Channel.Name} ({ctx.Guild.Name})",
                LogLevel.Info);

            var vScreen = SystemInformation.VirtualScreen;
            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromUnicode("📸"));
            using (var bm = new Bitmap(vScreen.Width,vScreen.Height))
            {
                using (var g = Graphics.FromImage(bm))
                {
                    g.CopyFromScreen(vScreen.Left, vScreen.Top, 0,0,bm.Size);
                }
                bm.Save(_liveFilename);
            }

            if (Config.Instance.BlurAmount > 0)
            {
                using (var img = await Image.LoadAsync(_liveFilename))
                {
                    img.Mutate(x => x.GaussianBlur((float) Config.Instance.BlurAmount));
                    await img.SaveAsync(_liveFilename);
                }
            }

            StalkbotClient.UpdateLastMessage(await ctx.RespondWithFileAsync(_liveFilename));
            File.Delete(_liveFilename);
        }
        internal static async Task TestScreenshotAsync()
        {
            var vScreen = SystemInformation.VirtualScreen;
            using (var bm = new Bitmap(vScreen.Width, vScreen.Height))
            {
                using (var g = Graphics.FromImage(bm))
                {
                    g.CopyFromScreen(vScreen.Left, vScreen.Top, 0, 0, bm.Size);
                }
                bm.Save(_testFilename);
            }

            if (Config.Instance.BlurAmount > 0)
            {
                using (var img = await Image.LoadAsync(_testFilename))
                {
                    img.Mutate(x => x.GaussianBlur((float)Config.Instance.BlurAmount));
                    await img.SaveAsync(_testFilename);
                }
            }
            Process.Start(_testFilename);
            await Task.Delay(500);
            File.Delete(_testFilename);
        }
    }
}
