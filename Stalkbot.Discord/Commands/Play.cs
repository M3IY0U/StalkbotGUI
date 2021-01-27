using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using NAudio.Wave;
using StalkbotGUI.Stalkbot.Utilities;
using YoutubeDLSharp;
using YoutubeDLSharp.Options;
using Timer = System.Timers.Timer;

namespace StalkbotGUI.Stalkbot.Discord.Commands
{
    public class Play : AlertCommand
    {
        /// <summary>
        /// Plays a file or url for the user
        /// </summary>
        /// <param name="ctx">Context this command has been executed in</param>
        /// <param name="url">The url the user wants played</param>
        /// <returns>The built task</returns>
        [RequireEnabled, Command("play"), Cooldown(1, 5, CooldownBucketType.Global),
        Description("Play a file or url. Can be attachment, file url or youtube link.")]
        public async Task PlayTask(CommandContext ctx, string url = "")
        {
            if (string.IsNullOrEmpty(url) && !ctx.Message.Attachments.Any())
            {
                Logger.Log($"{ctx.User.Username} used play without providing a source", LogLevel.Warning);
                return;
            }
            
            if (string.IsNullOrEmpty(url) && ctx.Message.Attachments.Any())
                url = ctx.Message.Attachments.First().Url;

            Logger.Log($"Play requested by {ctx.User.Username} in #{ctx.Channel.Name} ({ctx.Guild.Name})",
                LogLevel.Info);
            // file is being processed
            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromUnicode("♨"));

            if (!await Download(url))
                throw new Exception("Error downloading audio");
            await ConvertAudio("temp.wav");
            
            //done processing
            await ctx.Message.DeleteOwnReactionAsync(DiscordEmoji.FromUnicode("♨"));

            // start playing
            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromUnicode("▶"));
            if (!await PlayAudio()) // timeout limit hit
                await ctx.Message.CreateReactionAsync(DiscordEmoji.FromUnicode("🛑"));
            else // went smoothly
                await ctx.Message.CreateReactionAsync(DiscordEmoji.FromUnicode("✅"));
            
            try
            {
                await ctx.Message.DeleteOwnReactionAsync(DiscordEmoji.FromUnicode("▶"));
                File.Delete("temp.wav"); 
                File.Delete("final.wav");
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// Plays audio file while respecting timeout set in config
        /// </summary>
        /// <returns>The built task</returns>
        private static async Task<bool> PlayAudio()
        {
            // flag if timeout was hit
            var abort = false;
            // timeout timer
            var timer = new Timer
            {
                Interval = Config.Instance.Timeout,
                AutoReset = false
            };

            // audio playback boilerplate
            var outputDevice = new WaveOutEvent();
            using (var audioFile = new AudioFileReader("final.wav"))
            {
                outputDevice.Init(audioFile);
                // if there's a timeout set
                // hook the elapsed event to stop the player
                // and start the timer
                if (Config.Instance.Timeout > 0)
                {
                    timer.Elapsed += (sender, args) =>
                    {
                        abort = true;
                        outputDevice.Stop();
                    };
                }

                // dispose of the timer on playback end
                outputDevice.PlaybackStopped += (sender, args) =>
                {
                    timer.Stop();
                    timer.Dispose();
                };

                // actually start playing the audio 
                timer.Start();
                outputDevice.Play();
                while (outputDevice.PlaybackState == PlaybackState.Playing)
                    Thread.Sleep(1000);
            }
            return await Task.FromResult(!abort);
        }
        
        /// <summary>
        /// Downloads a file from anywhere youtube-dl supports and converts it to wav
        /// </summary>
        /// <param name="url">The url to download from</param>
        /// <returns>A boolean indicating whether the download was successful</returns>
        private static async Task<bool> Download(string url)
        {
            var ytdl = new YoutubeDL()
            {
                FFmpegPath = "ffpmeg.exe",
                YoutubeDLPath = "youtube-dl.exe",
                IgnoreDownloadErrors = false,
                OutputFileTemplate = "temp.%(ext)s",
                OverwriteFiles = true
            };

            var res = await ytdl.RunAudioDownload(url, AudioConversionFormat.Wav);
            return await Task.FromResult(res.Success);
        }

        /// <summary>
        /// Normalizes audio volume using ffmpeg to combat loud files
        /// </summary>
        /// <param name="filename">The filename to convert</param>
        /// <returns>The built task</returns>
        private static Task ConvertAudio(string filename)
        {
            using (var exeProcess = Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                Arguments = $"-y -i {filename} -af volume=-25dB,loudnorm=tp=0 -ar 44100 -ac 2 final.wav",
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true
            }))
            {
                exeProcess?.WaitForExit();
            }

            return Task.CompletedTask;
        }
    }
}
