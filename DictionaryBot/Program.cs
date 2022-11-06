using DictionaryBot.EventHandlers;
using DSharpPlus;
using DSharpPlus.Entities;

#if DEBUG
foreach (var line in File.ReadAllLines("settings.env"))
{
    Environment.SetEnvironmentVariable(line[..line.IndexOf('=')], line[(line.IndexOf('=') + 1)..]);
}
#endif

DiscordClient client = new(new DiscordConfiguration()
{
    Token = Environment.GetEnvironmentVariable("DISCORD_TOKEN") ?? throw new Exception("Please set DISCORD_TOKEN EnvVar!"),
    Intents = DiscordIntents.MessageContents | DiscordIntents.GuildMessages
});

client.MessageCreated += DictionaryEventHandler.MessageCreated;
client.GuildDownloadCompleted += DictionaryEventHandler.GuildDownloadCompleted;

await client.ConnectAsync(new DiscordActivity("with words"));

await Task.Delay(-1);