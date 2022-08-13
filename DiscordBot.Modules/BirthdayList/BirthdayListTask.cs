using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.Framework.Contract.TimedAction;

namespace DiscordBot.Modules.BirthdayList;

public class BirthdayListTask : ITimedAction
{
    private readonly BirthdayListManager _manager;

    public BirthdayListTask(BirthdayListManager manager)
    {
        _manager = manager;
    }

    public ExecutionTime GetExecutionTime()
    {
        return ExecutionTime.Daily;
    }

    public async Task ExecuteAsync(DiscordSocketClient client)
    {
        await _manager.RemoveBirthdayRolesFromUsers();
        await _manager.RefreshAllBirthdayChannel();
        await _manager.CheckNewBirthday();
    }
}