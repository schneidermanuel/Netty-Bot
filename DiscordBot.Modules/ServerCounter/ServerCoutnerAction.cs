using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Framework.Contract.TimedAction;

namespace DiscordBot.Modules.ServerCounter;

internal class ServerCoutnerAction : ITimedAction
{
    public ExecutionTime GetExecutionTime()
    {
        return ExecutionTime.Hourly;
    }

    public async Task ExecuteAsync(DiscordSocketClient client)
    {
        var guildCount = client.Guilds.Count;
        var message = $"We now support Slash Commands. Type / to see a list of available commands. Hosted with love by Brainy for {guildCount} Servers";
        await client.SetActivityAsync(new Game(message));
    }
}
