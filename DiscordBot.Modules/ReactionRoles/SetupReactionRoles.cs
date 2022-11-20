using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract.ReactionRoles;
using DiscordBot.Framework.Contract.TimedAction;

namespace DiscordBot.Modules.ReactionRoles;

public class SetupReactionRoles : ITimedAction
{
    private readonly ReactionRoleManager _manager;
    private readonly IReactionRoleDomain _domain;

    public SetupReactionRoles(ReactionRoleManager manager, IReactionRoleDomain domain)
    {
        _manager = manager;
        _domain = domain;
    }

    public ExecutionTime GetExecutionTime()
    {
        return ExecutionTime.PostBoot;
    }

    public async Task ExecuteAsync(DiscordSocketClient client)
    {
        foreach (var reactionRole in _manager.ReactionRoles)
        {
            try
            {
                await ProcessReactionRole(reactionRole, client);
            }
            catch (NullReferenceException e)
            {
                await _domain.DeleteReactionRoleAsync(reactionRole.Id);
            }
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

    private async Task ProcessReactionRole(ReactionRole reactionRole, DiscordSocketClient client)
    {
        var guild = client.GetGuild(reactionRole.GuildId);
        var message =
            await ((ISocketMessageChannel) guild.GetChannel(reactionRole.ChannelId))
                .GetMessageAsync(reactionRole.MessageId);
        var emote = reactionRole.Emote;
        await message.RemoveAllReactionsForEmoteAsync(emote);
        await Task.Delay(2000);
        await message.AddReactionAsync(emote);
    }
}