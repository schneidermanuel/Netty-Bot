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
        var message = "Sorry for the recent instability and thanks for trusting in netty-bot <3";
        await client.SetActivityAsync(new Game(message));
    }
}
