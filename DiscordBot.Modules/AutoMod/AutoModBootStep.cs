using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.Framework.Contract.TimedAction;

namespace DiscordBot.Modules.AutoMod;

internal class AutoModBootStep : ITimedAction
{
    private readonly AutoModManager _manager;

    public AutoModBootStep(AutoModManager manager)
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