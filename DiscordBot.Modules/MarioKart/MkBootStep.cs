using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract.MkCalculator;
using DiscordBot.Framework.Contract.TimedAction;

namespace DiscordBot.Modules.MarioKart;

internal class MkBootStep : ITimedAction
{
    private readonly IMkGameBusinessLogic _businessLogic;
    public ExecutionTime GetExecutionTime() => ExecutionTime.PostBoot;

    public MkBootStep(IMkGameBusinessLogic businessLogic)
    {
        _businessLogic = businessLogic;
    }

    public async Task Execute(DiscordSocketClient client)
    {
        await _businessLogic.ClearAllAsync();
    }
}