using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using StalkbotGUI.Stalkbot.Utilities;
using Image = SixLabors.ImageSharp.Image;

namespace StalkbotGUI.Stalkbot.Discord.Commands
{
    public class Screenshot : BaseCommandModule
    {
        private const string Filename = "screenshot.png";

        /// <summary>
        /// Captures a screenshot of all monitors
        /// </summary>
        /// <param name="ctx">Context this command has been executed in</param>
        /// <returns>The built task</returns>
        [RequireEnabled, Command("screenshot"), Aliases("ss"), 
         Cooldown(1, 5, CooldownBucketType.Global), Description("Captures a screenshot.")]
        public async Task CaptureScreen(CommandContext ctx)
        {
            var vScreen = SystemInformation.VirtualScreen;

            using (var bm = new Bitmap(vScreen.Width,vScreen.Height))
            {
                using (var g = Graphics.FromImage(bm))
                {
                    g.CopyFromScreen(vScreen.Left, vScreen.Top, 0,0,bm.Size);
                }
                bm.Save(Filename);
            }

            if (Config.Instance.BlurAmount > 0)
            {
                using (var img = await Image.LoadAsync(Filename))
                {
                    img.Mutate(x => x.GaussianBlur((float) Config.Instance.BlurAmount));
                    await img.SaveAsync(Filename);
                }
            }

            StalkbotClient.UpdateLastMessage(await ctx.RespondWithFileAsync(Filename));
            File.Delete(Filename);
        }
    }
}
