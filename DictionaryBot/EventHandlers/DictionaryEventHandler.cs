using DatabaseAccess.DbContext;
using DatabaseAccess.Models;
using DictionaryApiAccess;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace DictionaryBot.EventHandlers
{
    internal class DictionaryEventHandler
    {
        private static readonly Dictionary<ulong, string> _lastWordCache = [];
        private static readonly Dictionary<ulong, ulong> _lastUserCache = [];

        internal static Task GuildAvailable(DiscordClient _1, GuildCreatedEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                using DatabaseContext db = new();

                var dbGuild = db.Guilds.Find(e.Guild.Id);
                if (dbGuild is null)
                {
                    //create guilds in db if not already present
                    db.Add(new Guild() { Id = e.Guild.Id });
                    db.SaveChanges();
                    return;
                }

                //find guild dict game channel
                var channelId = dbGuild?.DictionaryGameChannel;
                if (channelId is null)
                    return;

                //cache last (hopefully) valid message
                var channel = await e.Guild.GetChannelAsync((ulong)channelId);
                var messages = channel.GetMessagesAsync(20);
                var message = await messages.FirstAsync(x => !x.Content.Trim().StartsWith('.'));
                _lastWordCache[e.Guild.Id] = message.Content.Trim();
                if (message.Author is null)
                    return;
                _lastUserCache[e.Guild.Id] = message.Author.Id;
            });
            return Task.CompletedTask;
        }

        internal static Task GuildCreated(DiscordClient _1, GuildCreatedEventArgs e)
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
                var channel = await e.Guild.GetChannelAsync((ulong)channelId);
                var messages = channel.GetMessagesAsync(20);
                var message = await messages.FirstAsync(x => !x.Content.Trim().StartsWith('.'));
                _lastWordCache[e.Guild.Id] = message.Content.Trim();
                if (message.Author is null)
                    return;
                _lastUserCache[e.Guild.Id] = message.Author.Id;
            });
            return Task.CompletedTask;
        }

        internal static Task MessageCreated(DiscordClient _1, MessageCreatedEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                if (e.Author.IsBot)
                    return;
                if (e.Message.Content.StartsWith('.'))
                    return;
                using DatabaseContext db = new();
                if (e.Channel.Id != db.Guilds.Find(e.Guild.Id)?.DictionaryGameChannel)
                    return;


                if (e.Message.Author is not null && e.Message.Author.Id == _lastUserCache[e.Guild.Id])
                {
                    await e.Message.DeleteAsync(); //delete message
                    var msg = await e.Channel.SendMessageAsync($"{e.Author.Mention} wait for someone else to send a message!"); //inform user that he has to wait 
                    await Task.Delay(2000); //wait two seconds (i hope)
                    await msg.DeleteAsync(); //delete information again
                    return;
                }

                if (e.Message.Content.Trim().Contains(' ')) //check for spaces in the message, if there are none move on
                {
                    await e.Message.DeleteAsync(); //delete message
                    var msg = await e.Channel.SendMessageAsync($"{e.Author.Mention} your message cannot have spaces!"); //inform user message cant have whitespaces
                    await Task.Delay(2000); //wait two seconds (i hope)
                    await msg.DeleteAsync(); //delete information again
                    return;
                }

                if (!string.IsNullOrWhiteSpace(_lastWordCache[e.Guild.Id]))
                {
                    if (!e.Message.Content.Trim().ToLower().StartsWith(_lastWordCache[e.Guild.Id].ToLower().Last()))
                    {
                        await e.Message.DeleteAsync(); //delete message
                        var msg = await e.Channel.SendMessageAsync($"{e.Author.Mention} the word {e.Message.Content.Trim()} does not start with an {_lastWordCache.Last()}!"); //inform user that chars have to match
                        await Task.Delay(2000); //wait two seconds (i hope)
                        await msg.DeleteAsync(); //delete information again
                        return;
                    }
                }

                var wordModel = await ApiAccess.IsWordValid(e.Message.Content);
                if (!wordModel)
                {
                    await e.Message.DeleteAsync(); //delete message
                    var msg = await e.Channel.SendMessageAsync($"{e.Author.Mention} the \"word\" {e.Message.Content.Trim()} has not been found in the dictionary!"); //inform user that word was not found
                    await Task.Delay(2000); //wait two seconds (i hope)
                    await msg.DeleteAsync(); //delete information again
                    return;
                }

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

                _lastWordCache[e.Guild.Id] = e.Message.Content.Trim(); //populate cache
                if (e.Message.Author is null)
                    return;
                _lastUserCache[e.Guild.Id] = e.Message.Author.Id;
            });
            return Task.CompletedTask;
        }
    }
}
