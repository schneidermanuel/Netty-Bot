using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.Framework.Contract.TimedAction;

namespace DiscordBot.Modules.MusicPlayer;

public class MusicBootStep : ITimedAction
{
    private readonly MusicManager _manager;

    public MusicBootStep(MusicManager manager)
    {
        _manager = manager;
    }

    public ExecutionTime GetExecutionTime()
    {
        return ExecutionTime.PostBoot;
    }

    public async Task ExecuteAsync(DiscordSocketClient client)
    {
        await _manager.Initialize();
        await Task.CompletedTask;
    }
}