using DatabaseAccess.DbContext;
using DatabaseAccess.Models;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace DictionaryBot.EventHandlers
{
    internal class DictionaryEventHandler
    {
        private static string _lastWordCache = "";

        internal static Task GuildAvailable(DiscordClient sender, GuildCreateEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                using DatabaseContext db = new();

                var dbGuild = db.Guilds.Find(e.Guild.Id);
                if (dbGuild is null)
                {
                    //create guilds in db on join
                    db.Add(new Guild() { Id = e.Guild.Id });
                    db.SaveChanges();
                    return;
                }

                //find guild dict game channel
                var channelId = dbGuild?.DictionaryGameChannel;
                if (channelId is null)
                    return;

                //cache last (hopefully) valid message
                var channel = e.Guild.GetChannel((ulong)channelId);
                var messages = await channel.GetMessagesAsync(20);
                _lastWordCache = messages.First(x => !x.Content.Trim().StartsWith(".")).Content.Trim();
            });
            return Task.CompletedTask;
        }

        internal static Task MessageCreated(DiscordClient sender, MessageCreateEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                if (e.Author.IsBot)
                    return;

                using DatabaseContext db = new();
                if (e.Channel.Id != db.Guilds.Find(e.Guild.Id)?.DictionaryGameChannel)
                    return;

                if (e.Message.Content.StartsWith("."))
                    return;
                if (e.Message.Content.Trim().IndexOf(' ') != -1) //check for spaces in the message, if there are none move on
                {
                    await e.Message.DeleteAsync(); //delete message
                    var msg = await e.Channel.SendMessageAsync($"{e.Author.Mention} your message cannot have spaces!"); //inform user message cant have whitespaces
                    await Task.Delay(2000); //wait two seconds (i hope)
                    await msg.DeleteAsync(); //delete information again
                    return;
                }

                if (!string.IsNullOrWhiteSpace(_lastWordCache))
                {
                    if (!e.Message.Content.Trim().StartsWith(_lastWordCache.Last()))
                    {
                        await e.Message.DeleteAsync(); //delete message
                        var msg = await e.Channel.SendMessageAsync($"{e.Author.Mention} the word {e.Message.Content.Trim()} does not start with an {_lastWordCache.Last()}!"); //inform user that chars have to match
                        await Task.Delay(2000); //wait twp seconds (i hope)
                        await msg.DeleteAsync(); //delete information again
                        return;
                    }
                }

                //validate word (https://dictionaryapi.dev/) ??? maybe find another way to do that, during development this api had some DNS issues

                if (db.DictionaryEntries.Any(x => x.Word == e.Message.Content.Trim())) //if word is present in db
                {
                    await e.Message.DeleteAsync(); //delete message
                    var msg = await e.Channel.SendMessageAsync($"{e.Author.Mention} the word {e.Message.Content.Trim()} has been written before!"); //inform user that word was used already
                    await Task.Delay(2000); //wait two seconds (i hope)
                    await msg.DeleteAsync(); //delete information again
                    return;
                }

                await e.Message.CreateReactionAsync(DiscordEmoji.FromUnicode("✅")); //add reaction

                db.Add(new DictionaryEntry(e.Message.Content.Trim()) { GuildId = e.Guild.Id }); //populate db
                db.SaveChanges();

                _lastWordCache = e.Message.Content.Trim(); //populate cache
            });
            return Task.CompletedTask;
        }
    }
}
