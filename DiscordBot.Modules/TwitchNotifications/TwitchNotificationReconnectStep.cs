using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.Framework.Contract.TimedAction;
using DiscordBot.PubSub.Twitch;

namespace DiscordBot.Modules.TwitchNotifications;

internal class TwitchNotificationReconnectStep : ITimedAction
{
    private readonly ITwitchPubsubManager _pubsubManager;

    public TwitchNotificationReconnectStep(ITwitchPubsubManager pubsubManager)
    {
        _pubsubManager = pubsubManager;
    }
    public ExecutionTime GetExecutionTime()
    {
        return ExecutionTime.Daily;
    }

    public async Task ExecuteAsync(DiscordSocketClient client)
    {
        await _pubsubManager.ReconnectAsync();
    }
}