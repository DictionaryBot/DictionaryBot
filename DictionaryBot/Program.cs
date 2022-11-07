using DictionaryBot.EventHandlers;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System.Reflection;
using ErrorEventHandler = DictionaryBot.EventHandlers.ErrorEventHandler;

#if DEBUG
foreach (var line in File.ReadAllLines("settings.env"))
{
    Environment.SetEnvironmentVariable(line[..line.IndexOf('=')], line[(line.IndexOf('=') + 1)..]);
}
#endif

DiscordClient client = new(new DiscordConfiguration()
{
    Token = Environment.GetEnvironmentVariable("DISCORD_TOKEN") ?? throw new Exception("Please set DISCORD_TOKEN EnvVar!"),
    Intents = DiscordIntents.MessageContents | DiscordIntents.GuildMessages | DiscordIntents.Guilds
});

client.MessageCreated += DictionaryEventHandler.MessageCreated;
client.GuildAvailable+= DictionaryEventHandler.GuildAvailable;

var slashCommands = client.UseSlashCommands();

slashCommands.SlashCommandErrored += ErrorEventHandler.SlashCommandErrored;
#if DEBUG
slashCommands.RegisterCommands(Assembly.GetExecutingAssembly(), 512370308532142091);
#else
slashCommands.RegisterCommands(Assembly.GetExecutingAssembly()); 
#endif

await client.ConnectAsync(new DiscordActivity("with words"));

await Task.Delay(-1);