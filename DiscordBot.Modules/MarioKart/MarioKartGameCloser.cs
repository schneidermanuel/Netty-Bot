using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract.MkCalculator;
using DiscordBot.Framework.Contract.TimedAction;

namespace DiscordBot.Modules.MarioKart;

internal class MarioKartGameCloser : ITimedAction
{
    private readonly MkGameManager _manager;

    public MarioKartGameCloser(MkGameManager manager)
    {
        _manager = manager;
    }

    public ExecutionTime GetExecutionTime()
    {
        return ExecutionTime.Hourly;
    }

    public async Task ExecuteAsync(DiscordSocketClient client)
    {
        var dueDate = DateTime.Now.AddHours(-3);
        await _manager.AutoCompleteGamesAsync(dueDate);
    }
}