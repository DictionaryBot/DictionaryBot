using DictionaryBot.EventHandlers;
using DSharpPlus;
using DSharpPlus.Commands;
using Microsoft.Extensions.Logging;
using System.Reflection;
using UptimeKumaHeartbeat;
using ErrorEventHandler = DictionaryBot.EventHandlers.ErrorEventHandler;

#if DEBUG
foreach (var line in File.ReadAllLines("settings.env"))
{
    Environment.SetEnvironmentVariable(line[..line.IndexOf('=')], line[(line.IndexOf('=') + 1)..]);
}
#endif

DiscordClient client = DiscordClientBuilder.CreateDefault(
    Environment.GetEnvironmentVariable("DISCORD_TOKEN") ?? throw new Exception("Please set DISCORD_TOKEN EnvVar!"),
    DiscordIntents.MessageContents | DiscordIntents.GuildMessages | DiscordIntents.Guilds
)
    .SetLogLevel(LogLevel.Debug)
    .ConfigureEventHandlers(b =>
        b.HandleMessageCreated(DictionaryEventHandler.MessageCreated)
        .HandleGuildAvailable(DictionaryEventHandler.GuildAvailable)
        .HandleGuildCreated(DictionaryEventHandler.GuildCreated)
    )
    .UseCommands((serviceProvider, extension) =>
    {
        extension.AddCommands(Assembly.GetExecutingAssembly());
        extension.CommandErrored += ErrorEventHandler.SlashCommandErrored;
    },
    new CommandsConfiguration
    {
        RegisterDefaultCommandProcessors = true,
        UseDefaultCommandErrorHandler = false
    })
    .Build();

await client.ConnectAsync();

var useKuma = Environment.GetEnvironmentVariable("USE_UPTIMEKUMA");
if (useKuma is not null && bool.Parse(useKuma))
{
    var heartbeatData = new HeartbeatData("", "");
    var heartbeatManager = new HeartbeatManager();
    await heartbeatManager.StartHeartbeatsAsync(Environment.GetEnvironmentVariable("UPTIMEKUMA_URL") ?? throw new Exception("Please set UPTIMEKUMA_URL EnvVar!"), heartbeatData);

}
await Task.Delay(-1);