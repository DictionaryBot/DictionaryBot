using DatabaseAccess;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace DictionaryBot.EventHandlers
{
    internal class DictionaryEventHandler
    {
        private static string _lastWordCache = "";

        internal static Task GuildDownloadCompleted(DiscordClient sender, GuildDownloadCompletedEventArgs e)
        {
            _ = Task.Run(() =>
            {
                //populate cache on startup
            });
            return Task.CompletedTask;
        }

        internal static Task MessageCreated(DiscordClient sender, MessageCreateEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                if (e.Author.IsBot)
                    return;
                
                //check if channel is correct (add db table && setup command)
                
                if (e.Message.Content.StartsWith("."))
                    return;
                if (e.Message.Content.Trim().IndexOf(' ') != -1)
                {
                    await e.Message.DeleteAsync(); //delete message
                    var msg = await e.Channel.SendMessageAsync($"{e.Author.Mention} your message cannot have spaces!"); //inform user message cant have whitespaces
                    await Task.Delay(1000); //wait one second (i hope)
                    await msg.DeleteAsync(); //delete information again
                    return;
                }

                if (!string.IsNullOrWhiteSpace(_lastWordCache))
                {
                    if (e.Message.Content.Trim().StartsWith(_lastWordCache.Last()))
                    {
                        await e.Message.DeleteAsync(); //delete message
                        var msg = await e.Channel.SendMessageAsync($"{e.Author.Mention} the word {e.Message.Content.Trim()} does not start with an {_lastWordCache.Last()}!"); //inform user that chars have to match
                        await Task.Delay(1000); //wait one second (i hope)
                        await msg.DeleteAsync(); //delete information again
                        return;
                    }
                }

                //call API to validate word (https://dictionaryapi.dev/)

                using DictionaryContext db = new();
                if (db.DictionaryEntries.Any(x => x.Word == e.Message.Content.Trim())) //if word is present in db
                {
                    await e.Message.DeleteAsync(); //delete message
                    var msg = await e.Channel.SendMessageAsync($"The word {e.Message.Content.Trim()} has been written before!"); //inform user that word was used already
                    await Task.Delay(1000); //wait one second (i hope)
                    await msg.DeleteAsync(); //delete information again
                    return;
                }

                await e.Message.CreateReactionAsync(DiscordEmoji.FromUnicode("✅"));

                _lastWordCache = e.Message.Content.Trim();
            });
            return Task.CompletedTask;
        }
    }
}
