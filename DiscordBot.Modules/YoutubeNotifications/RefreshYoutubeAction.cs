using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.Framework.Contract.TimedAction;

namespace DiscordBot.Modules.YoutubeNotifications;

internal class RefreshYoutubeAction : ITimedAction
{
    private readonly YoutubeNotificationManager _manager;

    public RefreshYoutubeAction(YoutubeNotificationManager manager)
    {
        _manager = manager;
    }
    public ExecutionTime GetExecutionTime()
    {
        return ExecutionTime.Daily;
    }

    public async Task ExecuteAsync(DiscordSocketClient client)
    {
        await _manager.RefreshAllRegistrations();
    }
}