using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using AForge.Video.DirectShow;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using StalkbotGUI.Stalkbot.Utilities;

namespace StalkbotGUI.Stalkbot.Discord.Commands
{
    public class Webcam : AlertCommand
    {
        /// <summary>
        /// Command for capturing a photo from the webcam
        /// </summary>
        /// <param name="ctx">Context this command has been executed in</param>
        /// <param name="camIndex">Index of the camera, uses config value if -1</param>
        /// <returns>The built task</returns>
        [RequireEnabled, Command("webcam"), Aliases("wc", "cam"), Cooldown(1, 5, CooldownBucketType.User),
         Description("Captures a photo from the webcam.")]
        public async Task WebcamTask(CommandContext ctx,
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
            try
            {
                capture.VideoResolution = TrySelectRes(capture, false);
            }
            catch (Exception ex) when (ex.HResult == -2146233079)
            { }
            catch (Exception)
            {
                throw;
            }

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
        /// Command for capturing a gif from the webcam
        /// </summary>
        /// <param name="ctx">Context this command has been executed in</param>
        /// <returns>The built task</returns>
        [RequireEnabled, Command("webcamgif"), Aliases("gif", "wcg", "wcgif"), Cooldown(1, 10, CooldownBucketType.Global),
         Description("Creates a gif from the webcam.")]
        public async Task GifTask(CommandContext ctx)
        {
            Directory.CreateDirectory("gif");
            Logger.Log($"Webcam Gif requested by {ctx.User.Username} in #{ctx.Channel.Name} ({ctx.Guild.Name})",
                LogLevel.Info);

            var capture =
                new VideoCaptureDevice(Constants.Cameras[Config.Instance.DefaultCam].MonikerString);
            try
            {
                capture.VideoResolution = TrySelectRes(capture, false);
            }
            catch (Exception ex) when (ex.HResult == -2146233079)
            { }
            catch (Exception)
            {
                throw;
            }

            capture.Start();
            await Task.Delay(Config.Instance.CamTimer);

            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromUnicode("🎥"));
            var timer = new Timer(Config.Instance.GifLength) { Enabled = true, AutoReset = false };
            timer.Elapsed += (sender, args) => capture.SignalToStop();

            var counter = 0;
            capture.NewFrame += (sender, args) =>
            {
                args.Frame.Save($"gif{Path.DirectorySeparatorChar}{counter++}.png", ImageFormat.Png);
                args.Frame.Dispose();
            };

            while (capture.IsRunning)
                capture.WaitForStop();
            capture.Stop();

            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromUnicode("📤"));
            await ctx.Message.DeleteOwnReactionAsync(DiscordEmoji.FromUnicode("🎥"));

            await CreateGif();
            StalkbotClient.UpdateLastMessage(await ctx.RespondWithFileAsync("result.gif"));
            await ctx.Message.DeleteOwnReactionAsync(DiscordEmoji.FromUnicode("📤"));
            Directory.Delete("gif", true);
            File.Delete("result.gif");
            timer.Dispose();
        }

        /// <summary>
        /// Creates a gif from an image sequence
        /// </summary>
        /// <returns>The built task</returns>
        private static Task CreateGif()
        {
            var fps = Config.Instance.GifFps
                ? $"-r {Directory.GetFiles("gif").Length / (Config.Instance.GifLength / 1000)}"
                : "";
            
            using (var exeProcess = Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                Arguments = $"-y {fps} -i gif{Path.DirectorySeparatorChar}%d.png -vf \"scale=400:-1\" result.gif",
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true
            }))
            {
                exeProcess?.WaitForExit();
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Tries to set the resolution to the config values, otherwise returns highest option
        /// </summary>
        /// <param name="device">The device to check</param>
        /// <param name="isGif">Whether gif res or normal res should be used</param>
        /// <returns>The desired capabilities/resolution</returns>
        private static VideoCapabilities TrySelectRes(VideoCaptureDevice device, bool isGif)
        {
            var width = isGif
                ? Config.Instance.GifCamWidth
                : Config.Instance.CamWidth;

            var height = isGif
                ? Config.Instance.GifCamHeight
                : Config.Instance.CamHeight;

            foreach (var cap in device.VideoCapabilities)
            {
                if (cap.FrameSize.Width == width && cap.FrameSize.Height == height)
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
