using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.Framework.Contract.TimedAction;

namespace DiscordBot.Modules.TwitterNotification;

internal class TwitterApiInitializer : ITimedAction
{
    private readonly TwitterStreamManager _manager;

    public TwitterApiInitializer(TwitterStreamManager manager)
    {
        _manager = manager;
    }
    
    public ExecutionTime GetExecutionTime()
    {
        return ExecutionTime.PostBoot;
    }

    public async Task ExecuteAsync(DiscordSocketClient client)
    {
        await _manager.InitializeAsync();
    }
}