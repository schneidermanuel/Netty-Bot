using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract.MkCalculator;
using DiscordBot.Framework.Contract.TimedAction;

namespace DiscordBot.Modules.MarioKart;

internal class MkBootStep : ITimedAction
{
    private readonly IMkGameDomain _domain;
    public ExecutionTime GetExecutionTime() => ExecutionTime.PostBoot;

    public MkBootStep(IMkGameDomain domain)
    {
        _domain = domain;
    }

    public async Task ExecuteAsync(DiscordSocketClient client)
    {
        await _domain.ClearAllAsync();
    }
}