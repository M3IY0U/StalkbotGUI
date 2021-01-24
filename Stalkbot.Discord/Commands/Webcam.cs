using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AForge.Video.DirectShow;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using StalkbotGUI.Stalkbot.Utilities;

namespace StalkbotGUI.Stalkbot.Discord.Commands
{
    public class Webcam : BaseCommandModule
    {
        /// <summary>
        /// Command for capturing a photo from the webcam
        /// </summary>
        /// <param name="ctx">Context this command has been executed in</param>
        /// <param name="camIndex">Index of the camera, uses config value if -1</param>
        /// <returns>The built task</returns>
        [RequireEnabled, Command("webcam"), Aliases("wc", "cam"), Cooldown(1, 5, CooldownBucketType.User),
         Description("Captures a photo from the webcam.")]
        public async Task CamCommand(CommandContext ctx,
            [Description("Index of the cam you want to capture.\nUse the webcam command to list them.")] int camIndex = -1)
        {
            // if no camera index has been passed, use the one from the config
             if (camIndex == -1)
                camIndex = Config.Instance.DefaultCam;

             Logger.Log($"Webcam requested by {ctx.User.Username} in #{ctx.Channel.Name} ({ctx.Guild.Name})",
                 LogLevel.Info);

             // init capture
            var capture =
                new VideoCaptureDevice(Constants.Cameras[camIndex].MonikerString);
            capture.VideoResolution = TrySelectRes(capture);

            capture.Start();
            
            // wait for configured time
            await Task.Delay(Config.Instance.CamTimer);
            // capture photo
            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromUnicode("📸"));
            capture.NewFrame += (sender, args) =>
            {
                args.Frame.Save("webcam.png");
                capture.SignalToStop();
            };

            // stop capture
            while (capture.IsRunning)
                capture.WaitForStop();
            capture.Stop();
            // send/update message and delete file from disk
            StalkbotClient.UpdateLastMessage(await ctx.RespondWithFileAsync("webcam.png"));
            File.Delete("webcam.png");
        }

        /// <summary>
        /// Tries to set the resolution to the config values, otherwise returns highest option
        /// </summary>
        /// <param name="device">The device to check</param>
        /// <returns>The desired capabilities/resolution</returns>
        private static VideoCapabilities TrySelectRes(VideoCaptureDevice device)
        {
            foreach (var cap in device.VideoCapabilities)
            {
                if (cap.FrameSize.Width == Config.Instance.CamWidth && cap.FrameSize.Height == Config.Instance.CamHeight) 
                    return cap;
            }
            return device.VideoCapabilities.Last();
        }

        /// <summary>
        /// Command that lists all webcams
        /// </summary>
        /// <param name="ctx">Context this command has been executed in.</param>
        /// <returns>The built task</returns>
        [RequireEnabled, Command("webcams"), Aliases("wcs"), Description("Lists all available webcams.")]
        public async Task ListCamsCommand(CommandContext ctx)
        {
            Logger.Log($"Webcam list requested by {ctx.User.Username} in #{ctx.Channel.Name} ({ctx.Guild.Name})", LogLevel.Info);
            var result = "**Available Webcams:**\n```\n";
            for (var i = 0; i < Constants.Cameras.Count; i++)
                result += $"{i} => {Constants.Cameras[i].Name}\n";
            StalkbotClient.UpdateLastMessage(await ctx.RespondAsync($"{result}\n```"));
        }
    }
}
