using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;
using Microsoft.Extensions.Logging;

namespace DictionaryBot.EventHandlers
{
    internal class ErrorEventHandler
    {
        internal static Task SlashCommandErrored(SlashCommandsExtension sender, SlashCommandErrorEventArgs e)
        {
            _ = Task.Run(() =>
            {
                sender.Client.Logger.Log(LogLevel.Error, e.Exception.Message, e.Exception.StackTrace);
            });
            return Task.CompletedTask;
        }
    }
}
