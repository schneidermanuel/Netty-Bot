using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.Framework.Contract.TimedAction;

namespace DiscordBot.Modules.AutoRole;

internal class SetupAutoRolesTask : ITimedAction
{
    private readonly AutoRoleManager _manager;

    public SetupAutoRolesTask(AutoRoleManager manager)
    {
        _manager = manager;
    }

    public ExecutionTime GetExecutionTime()
    {
        return ExecutionTime.PostBoot;
    }

    public async Task ExecuteAsync(DiscordSocketClient client)
    {
        await _manager.RefreshSetupsAsync();
        client.UserJoined += async user => { await _manager.UserJoinedGuild(user); };
    }
}