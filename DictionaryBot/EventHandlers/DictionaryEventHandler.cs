using DSharpPlus;
using DSharpPlus.EventArgs;

namespace DictionaryBot.EventHandlers
{
    internal class DictionaryEventHandler
    {
        internal static Task MessageCreated(DiscordClient sender, MessageCreateEventArgs e)
        {
            _ = Task.Run(() =>
            {
                //ignore messages with a certain prefix

                //call API to validate word (https://dictionaryapi.dev/)

                //check database for repeated words

                //react with a checkmark or send a message and delete the word
            });
            return Task.CompletedTask;
        }
    }
}
