using DatabaseAccess.DbContext;
using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Trees.Metadata;
using DSharpPlus.Entities;
using System.ComponentModel;

namespace DictionaryBot.SlashCommands
{
    public class SetupSlashCommands
    {
        [Command("DictionaryGame")]
        [AllowedProcessors<SlashCommandProcessor>()]
        [RequireGuild()]
        [Description("Set up the dictionary game")]
        [RequirePermissions(DiscordPermission.ManageGuild)]
        public async Task SetupDictGame(CommandContext ctx,
            [Description("What channel to set the game up in")]
            DiscordChannel channel)
        {
            await ctx.DeferResponseAsync(); //show a thinking state

            using DatabaseContext db = new();
            var dbGuild = db.Guilds.Find(ctx.Guild!.Id);
            if (dbGuild is null) // yeah idk what would make this happen, bot downtime maybe?
            {
                await ctx.EditResponseAsync("Something went wrong, please try reinviting the Bot!");
                return;
            }
            dbGuild.DictionaryGameChannel = channel.Id;
            db.SaveChanges();

            await ctx.EditResponseAsync($"Successfully set up {channel.Mention} for the dictionary game, run this command again to change the channel.");
        }
    }
}
