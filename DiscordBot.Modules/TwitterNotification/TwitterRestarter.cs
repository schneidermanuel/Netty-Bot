using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.Framework.Contract.TimedAction;

namespace DiscordBot.Modules.TwitterNotification;

internal class TwitterRestarter : ITimedAction
{
    private readonly TwitterStreamManager _streamManager;

    public TwitterRestarter(TwitterStreamManager streamManager)
    {
        _streamManager = streamManager;
    }

    public ExecutionTime GetExecutionTime()
    {
        return ExecutionTime.Hourly;
    }

    public async Task ExecuteAsync(DiscordSocketClient client)
    {
        await _streamManager.RestartAsync();
    }
}