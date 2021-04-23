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
        private const string LiveFilename = "screenshot.png";
        private const string TestFilename = "test_ss.png";

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

            await TakeScreenshot(LiveFilename);
            
            if (Config.Instance.BlurAmount > 0)
            {
                using (var img = await Image.LoadAsync(LiveFilename))
                {
                    img.Mutate(x => x.GaussianBlur((float)Config.Instance.BlurAmount));
                    await img.SaveAsync(LiveFilename);
                    //await img.SaveAsync(stream, JpegFormat.Instance);
                }
            }

            var msg = new DiscordMessageBuilder()
                .WithReply(ctx.Message.Id)
                .WithFile(new FileStream(LiveFilename, FileMode.Open));

            StalkbotClient.UpdateLastMessage(await ctx.RespondAsync(msg));
            File.Delete(LiveFilename);
        }

        internal static async Task TestScreenshotAsync()
        {
            await TakeScreenshot(TestFilename);

            if (Config.Instance.BlurAmount > 0)
            {
                using (var img = await Image.LoadAsync(TestFilename))
                {
                    img.Mutate(x => x.GaussianBlur((float)Config.Instance.BlurAmount));
                    await img.SaveAsync(TestFilename);
                }
            }
            Process.Start(TestFilename);
            await Task.Delay(500);
            File.Delete(TestFilename);
        }

        private static Task TakeScreenshot(string filename)
        {
            var vScreen = SystemInformation.VirtualScreen;
            using (var bm = new Bitmap(vScreen.Width, vScreen.Height))
            {
                using (var g = Graphics.FromImage(bm))
                {
                    g.CopyFromScreen(vScreen.Left, vScreen.Top, 0, 0, bm.Size);
                }
                bm.Save(filename);
            }

            return Task.CompletedTask;
        }
    }
}
