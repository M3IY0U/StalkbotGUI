using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using NAudio.Wave;
using StalkbotGUI.Stalkbot.Utilities;
using System.IO;
using System.Threading.Tasks;
using System.Timers;

namespace StalkbotGUI.Stalkbot.Discord.Commands
{
    public class Microphone : AlertCommand
    {
        [Command("mic"), RequireEnabled]
        public async Task MicTask(CommandContext ctx, int sampleRate = 44100)
        {
            await Task.Delay(Config.Instance.MicTimer);

            var mic = WaveIn.GetCapabilities(Config.Instance.MicIndex);
            var source = new WaveInEvent() { DeviceNumber = Config.Instance.MicIndex, WaveFormat = new WaveFormat(sampleRate, 1) };
            var writer = new WaveFileWriter("recording.wav", source.WaveFormat);
            var timer = new Timer { AutoReset = false, Interval = Config.Instance.MicLength };
            timer.Elapsed += async (sender, args) =>
            {
                source.StopRecording();
                await Task.Delay(500);
                await ctx.Message.DeleteOwnReactionAsync(DiscordEmoji.FromUnicode("🎙"));
                var msg = new DiscordMessageBuilder()
                    .WithReply(ctx.Message.Id)
                    .WithFile(new FileStream("recording.wav", FileMode.Open));

                StalkbotClient.UpdateLastMessage(await ctx.RespondAsync(msg));
                File.Delete("recording.wav");
            };

            Logger.Log($"Started recording with mic {mic.ProductName}", LogLevel.Info);

            source.DataAvailable += (sender, args) =>
            {
                if (writer == null) return;
                writer.Write(args.Buffer, 0, args.BytesRecorded);
                writer.Flush();
            };

            source.RecordingStopped += (sender, args) =>
            {
                if (source != null)
                {
                    source.StopRecording();
                    source.Dispose();
                    source = null;
                }

                if (writer == null)
                {
                    return;
                }

                writer.Dispose();
                writer = null;
            };
            timer.Start();
            source.StartRecording();
            await ctx.Message.CreateReactionAsync(DiscordEmoji.FromUnicode("🎙"));
        }
    }
}
