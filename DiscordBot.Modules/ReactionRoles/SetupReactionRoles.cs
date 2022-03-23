using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract.ReactionRoles;
using DiscordBot.Framework.Contract.TimedAction;

namespace DiscordBot.Modules.ReactionRoles;

public class SetupReactionRoles : ITimedAction
{
    private readonly ReactionRoleManager _manager;
    private readonly IReactionRoleBusinessLogic _businessLogic;

    public SetupReactionRoles(ReactionRoleManager manager, IReactionRoleBusinessLogic businessLogic)
    {
        _manager = manager;
        _businessLogic = businessLogic;
    }

    public ExecutionTime GetExecutionTime()
    {
        return ExecutionTime.PostBoot;
    }

    public async Task Execute(DiscordSocketClient client)
    {
        foreach (var reactionRole in _manager.ReactionRoles)
        {
            try
            {
                await ProcessReactionRole(reactionRole, client);
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e.Message + "\n" + e.StackTrace);
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
        await message.RemoveAllReactionsAsync();
        await Task.Delay(2000);
        await message.AddReactionAsync(emote);
    }
}