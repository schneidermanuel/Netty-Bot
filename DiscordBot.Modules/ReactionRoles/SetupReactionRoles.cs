using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract.ReactionRoles;
using DiscordBot.Framework.Contract.TimedAction;

namespace DiscordBot.Modules.ReactionRoles;

public class SetupReactionRoles : ITimedAction
{
    private readonly ReactionRoleManager _manager;

    public SetupReactionRoles(ReactionRoleManager manager)
    {
        _manager = manager;
    }

    public ExecutionTime GetExecutionTime()
    {
        return ExecutionTime.PostBoot;
    }

    public async Task ExecuteAsync(DiscordSocketClient client)
    {
        foreach (var reactionRole in _manager.ReactionRoles)
        {
            await _manager.ProcessReactionRole(reactionRole);
        }

        client.ReactionAdded += async (message, channel, reaction) =>
        {
            await _manager.ReactionAdded(message, reaction);
        };
        client.ReactionRemoved += async (message, channel, reaction) =>
        {
            await _manager.ReactionRemoved(message, reaction);
        };
    }
}