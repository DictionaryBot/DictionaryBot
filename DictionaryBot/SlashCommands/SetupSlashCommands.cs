using DatabaseAccess.DbContext;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace DictionaryBot.SlashCommands
{
    public class SetupSlashCommands : ApplicationCommandModule
    {
        [SlashCommand("DictionaryGame", "Set up the dictionary game")]
        [SlashCommandPermissions(Permissions.ManageGuild)]
        public async Task SetupDictGame(InteractionContext ctx,
            [Option("Channel", "What channel to set the game up in")]
            DiscordChannel channel)
        {
            await ctx.DeferAsync(true); //show a thinking state

            using DatabaseContext db = new();
            var dbGuild = db.Guilds.Find(ctx.Guild.Id);
            if (dbGuild is null) // yeah idk what would make this happen, bot downtime maybe?
            {
                await ctx.EditResponseAsync(new() { Content = "Something went wrong, please try reinviting the Bot!" });
                return;
            }
            dbGuild.DictionaryGameChannel = channel.Id;
            db.SaveChanges();

            await ctx.EditResponseAsync(new() { Content = $"Successfully set up {channel.Mention} for the dictionary game, run this command again to change the channel." });
        }
    }
}
