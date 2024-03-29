﻿using System;
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
            [Description("Index of the cam you want to capture.\nUse the webcam command to list them.")]
            int camIndex = -1)
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
            catch (Exception ex)
            {
                if (ex.HResult != -2146233079)
                    throw;
            }

            capture.Start();

            // wait for configured time
            await Task.Delay(Config.Instance.CamTimer);
            // capture photo
            await CommandHelper.TryAddFeedbackEmoji(DiscordEmoji.FromUnicode("📸"), ctx.Message);
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
            var msg = new DiscordMessageBuilder()
                .WithReply(ctx.Message.Id)
                .WithFile(new FileStream("webcam.png", FileMode.Open));
            StalkbotClient.UpdateLastMessage(await ctx.RespondAsync(msg));
            File.Delete("webcam.png");
        }

        /// <summary>
        /// Command for capturing a gif from the webcam
        /// </summary>
        /// <param name="ctx">Context this command has been executed in</param>
        /// <param name="comArgs">Params for making the gif: Custom FPS setting for single use sped up or slowed down gifs</param>
        /// <returns>The built task</returns>
        [RequireEnabled, Command("webcamgif"), Aliases("gif", "wcg", "wcgif"),
         Cooldown(1, 10, CooldownBucketType.Global),
         Description("Creates a gif from the webcam.")]
        public async Task GifTask(CommandContext ctx, [Description("FPS value to set the gif at")] params string[] comArgs)
        {
            Directory.CreateDirectory("gif");
            Logger.Log($"Webcam Gif requested by {ctx.User.Username} in #{ctx.Channel.Name} ({ctx.Guild.Name})",
                LogLevel.Info);

            var capture =
                new VideoCaptureDevice(Constants.Cameras[Config.Instance.DefaultCam].MonikerString);

            try
            {
                capture.VideoResolution = TrySelectRes(capture, true);
            }
            catch (Exception ex)
            {
                if (ex.HResult != -2146233079)
                    throw;
            }

            capture.Start();
            await Task.Delay(Config.Instance.CamTimer);

            await CommandHelper.TryAddFeedbackEmoji(DiscordEmoji.FromUnicode("🎥"), ctx.Message);
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

            await CommandHelper.TryAddFeedbackEmoji(DiscordEmoji.FromUnicode("📤"), ctx.Message);
            await CommandHelper.TryRemoveFeedbackEmoji(DiscordEmoji.FromUnicode("🎥"), ctx.Message);

            await CreateGif(comArgs);

            var msg = new DiscordMessageBuilder()
                .WithReply(ctx.Message.Id)
                .WithFile(new FileStream("result.gif", FileMode.Open));

            StalkbotClient.UpdateLastMessage(await ctx.RespondAsync(msg));
            await CommandHelper.TryRemoveFeedbackEmoji(DiscordEmoji.FromUnicode("📤"), ctx.Message);
            Directory.Delete("gif", true);
            File.Delete("result.gif");
            timer.Dispose();
        }

        /// <summary>
        /// Creates a gif from an image sequence
        /// </summary>
        /// <returns>The built task</returns>
        private static Task CreateGif(string[] args)
        {
            // check if an fps arg was sent
            var fps = args.Length > 0 ? Convert.ToInt32(args[0]) : 0; 
            var fpsString = "";
            if (Config.Instance.CustomGifFps > 0 || Config.Instance.GifFps || fps > 0)
            {
                if (fps == 0)
                {
                    fps = Config.Instance.GifFps ? Directory.GetFiles("gif").Length / (Config.Instance.GifLength / 1000) : Config.Instance.CustomGifFps;
                } else
                {
                    //clamp the value of custom argument fps, needs file amount as upper limit because we can't realistically go under 1s
                    var maxFps = Directory.GetFiles("gif").Length;
                    fps = Math.Max(1, Math.Min(fps, maxFps));
                }
                fpsString = $"-r {fps}";
            }
            
            using (var exeProcess = Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                Arguments = $"-y {fpsString} -i gif{Path.DirectorySeparatorChar}%d.png -vf \"scale=400:-1\" result.gif",
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
            Logger.Log($"Webcam list requested by {ctx.User.Username} in #{ctx.Channel.Name} ({ctx.Guild.Name})",
                LogLevel.Info);
            var result = "**Available Webcams:**\n```\n";
            for (var i = 0; i < Constants.Cameras.Count; i++)
                result += $"{i} => {Constants.Cameras[i].Name}\n";
            StalkbotClient.UpdateLastMessage(await ctx.RespondAsync($"{result}\n```"));
        }
    }
}