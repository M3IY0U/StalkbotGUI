using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using StalkbotGUI.Stalkbot.Utilities;

namespace StalkbotGUI.Stalkbot.Discord.Commands
{
    public class Clipboard : BaseCommandModule
    {
        /// <summary>
        /// Command for getting/setting clipboard text
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        [RequireEnabled, Command("clipboard"), Aliases("cp"), Cooldown(1, 10, CooldownBucketType.Global),
        Description("Get or set the user's clipboard.")]
        public async Task ClipboardTask(CommandContext ctx,
            [Description("The content to set. Leave empty to return users clipboard")][RemainingText] string content = "")
        {
            var text = "";
            var thread = string.IsNullOrEmpty(content)
                ? new Thread(() =>
                {
                    if (System.Windows.Clipboard.ContainsText())
                        text = System.Windows.Clipboard.GetText();
                })
                : new Thread(() =>
                {
                    for (var i = 0; i < 5; i++)
                    {
                        try
                        {
                            System.Windows.Clipboard.SetText(content, TextDataFormat.Text);
                            break;
                        }
                        catch { /* ignored */ }
                        Thread.Sleep(10);
                    }
                    ctx.Message.CreateReactionAsync(DiscordEmoji.FromUnicode("✅"));
                });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            if (!string.IsNullOrEmpty(text))
                await ctx.RespondAsync(text);
        }
    }
}
