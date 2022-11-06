using DictionaryBot.EventHandlers;
using DSharpPlus;

#if DEBUG
foreach (var line in File.ReadAllLines("settings.env"))
{
    Environment.SetEnvironmentVariable(line[..line.IndexOf('=')], line[(line.IndexOf('=') + 1)..]);
}
#endif

DiscordClient client = new(new DiscordConfiguration()
{
    Token = Environment.GetEnvironmentVariable("DISCORD_TOKEN") ?? throw new Exception("Please set DISCORD_TOKEN EnvVar!"),
    Intents = DiscordIntents.MessageContents
});

client.MessageCreated += DictionaryEventHandler.MessageCreated;

await client.ConnectAsync();

await Task.Delay(-1);