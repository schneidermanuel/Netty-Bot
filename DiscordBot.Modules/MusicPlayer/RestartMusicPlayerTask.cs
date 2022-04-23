using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.Framework.Contract.TimedAction;

namespace DiscordBot.Modules.MusicPlayer;

internal class RestartMusicPlayerTask : ITimedAction
{
    private readonly MusicManager _manager;

    public RestartMusicPlayerTask(MusicManager manager)
    {
        _manager = manager;
    }
    public ExecutionTime GetExecutionTime()
    {
        return ExecutionTime.Daily;
    }

    public async Task Execute(DiscordSocketClient client)
    {
        await _manager.RestartAsync();
    }
}