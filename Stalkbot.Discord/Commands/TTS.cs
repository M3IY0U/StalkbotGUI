using System.Speech.Synthesis;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using StalkbotGUI.Stalkbot.Utilities;
using Timer = System.Timers.Timer;

namespace StalkbotGUI.Stalkbot.Discord.Commands
{
    public class Tts : AlertCommand
    {
        /// <summary>
        /// Command used for playing tts audio for the user
        /// </summary>
        /// <param name="ctx">Context this command was executed in</param>
        /// <param name="input">The text to speak</param>
        /// <returns>The built task</returns>
        [RequireEnabled, Command("tts"), Aliases("say"),
         Description("Tell the user something via their installed TTS system.")]
        public async Task TtsTask(CommandContext ctx, [RemainingText] string input)
        {
            Logger.Log($"TTS requested by {ctx.User.Username} in #{ctx.Channel.Name} ({ctx.Guild.Name})",
                LogLevel.Info);
            var p = new Prompt(input);
            await CommandHelper.TryAddFeedbackEmoji(DiscordEmoji.FromUnicode("📣"), ctx.Message);

            if (!await SpeakAudio(p))
                await CommandHelper.TryAddFeedbackEmoji(DiscordEmoji.FromUnicode("🛑"), ctx.Message);
            else // went smoothly
                await CommandHelper.TryAddFeedbackEmoji(DiscordEmoji.FromUnicode("✅"), ctx.Message);

            await CommandHelper.TryRemoveFeedbackEmoji(DiscordEmoji.FromUnicode("📣"), ctx.Message);
        }

        /// <summary>
        /// Speaks the audio for the user
        /// </summary>
        /// <param name="prompt">Prompt containing the text</param>
        /// <returns>A boolean indicating whether the timeout was hit</returns>
        private Task<bool> SpeakAudio(Prompt prompt)
        {
            // setup
            var synth = new SpeechSynthesizer();
            synth.SetOutputToDefaultAudioDevice();
            var abort = false;
            var timer = new Timer
            {
                Enabled = true,
                Interval = Config.Instance.Timeout,
                AutoReset = false
            };

            // dispose timer on completion
            synth.SpeakCompleted += (sender, args) =>
            {
                timer.Stop();
                timer.Dispose();
            };

            // if there's a timeout set, hook the timer elapsed event
            if (Config.Instance.Timeout > 0)
            {
                timer.Elapsed += (sender, args) =>
                {
                    abort = true;
                    synth.Dispose();
                };
            }

            // disposing the synth while it's speaking will throw,
            // so when that happens abort gets set
            try
            {
                synth.Speak(prompt);
            }
            catch
            {
                abort = true;
            }
            return Task.FromResult(!abort);
        }
    }
}
