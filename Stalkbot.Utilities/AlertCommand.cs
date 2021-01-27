using System.IO;
using System.Media;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;

namespace StalkbotGUI.Stalkbot.Utilities
{
    public class AlertCommand : BaseCommandModule
    {
        /// <summary>
        /// Plays an alert if the command will be executed 
        /// </summary>
        /// <param name="ctx">Context this command has been executed in</param>
        /// <returns>The built task</returns>
        public override Task BeforeExecutionAsync(CommandContext ctx)
        {
            // if the command is disabled or there's no file, abort
            if (!File.Exists($"{ctx.Command.Name.ToLower()}.wav")) 
                return base.BeforeExecutionAsync(ctx);

            // play the audio
            using (var player = new SoundPlayer($"{ctx.Command.Name.ToLower()}.wav"))
                player.Play();

            return base.BeforeExecutionAsync(ctx);
        }
    }
}
