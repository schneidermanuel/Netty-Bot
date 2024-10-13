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
        var message = "https://brainyxs.com/";
        await client.SetActivityAsync(new Game(message));
    }
}
