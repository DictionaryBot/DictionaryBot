using DSharpPlus.Commands;
using DSharpPlus.Commands.EventArgs;
using Microsoft.Extensions.Logging;

namespace DictionaryBot.EventHandlers
{
    internal class ErrorEventHandler
    {
        internal static Task SlashCommandErrored(CommandsExtension sender, CommandErroredEventArgs e)
        {
            _ = Task.Run(() =>
            {
                sender.Client.Logger.Log(LogLevel.Error, e.Exception.Message, e.Exception.StackTrace);
            });
            return Task.CompletedTask;
        }
    }
}
