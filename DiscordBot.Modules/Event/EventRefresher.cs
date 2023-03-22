using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.Framework.Contract.TimedAction;

namespace DiscordBot.Modules.Event;

internal class EventRefresher : ITimedAction
{
    private readonly EventLifesycleManager _manager;

    public EventRefresher(EventLifesycleManager manager)
    {
        _manager = manager;
    }

    public ExecutionTime GetExecutionTime()
    {
        return ExecutionTime.Hourly;
    }

    public async Task ExecuteAsync(DiscordSocketClient client)
    {
        await _manager.Refresh();
    }
}