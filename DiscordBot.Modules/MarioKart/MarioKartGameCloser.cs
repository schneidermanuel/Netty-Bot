using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract.MkCalculator;
using DiscordBot.Framework.Contract.TimedAction;

namespace DiscordBot.Modules.MarioKart;

internal class MarioKartGameCloser : ITimedAction
{
    private readonly IMkGameDomain _domain;

    public MarioKartGameCloser(IMkGameDomain domain)
    {
        _domain = domain;
    }
    public ExecutionTime GetExecutionTime()
    {
        return ExecutionTime.Hourly;
    }

    public async Task ExecuteAsync(DiscordSocketClient client)
    {
        var dueDate = DateTime.Now.AddHours(-3);
        await _domain.AutoCompleteOldGames(dueDate);
    }
}