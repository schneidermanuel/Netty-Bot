using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract.ReactionRoles;

namespace DiscordBot.Modules.ReactionRoles;

public class ReactionRoleManager
{
    public List<ReactionRole> ReactionRoles;

    public async Task ReactionAdded(Cacheable<IUserMessage, ulong> message,
        SocketReaction reaction)
    {
        var reactionRoles = ReactionRoles.Where(role => role.MessageId == message.Id);
        foreach (var reactionRole in reactionRoles)
        {
            await ProcessReactionRoleAsync(message, reaction, reactionRole);
        }
    }

    private static async Task ProcessReactionRoleAsync(Cacheable<IUserMessage, ulong> message, SocketReaction reaction, ReactionRole reactionRole)
    {
        if (reaction.Emote.Name != reactionRole.Emote.Name)
        {
            return;
        }

        var loadedMessage = await message.GetOrDownloadAsync();
        var guild = ((SocketGuildChannel)loadedMessage.Channel).Guild;
        var user = guild.GetUser(reaction.UserId);
        if (user.IsBot)
        {
            return;
        }

        var role = guild.GetRole(reactionRole.RoleId);
        if (user.Roles.Any(x => x.Id == reactionRole.RoleId))
        {
            await user.RemoveRoleAsync(role);
            Console.WriteLine($"[Reaction role] Removed {role.Name} from {user.Username}");
        }
        else
        {
            await user.AddRoleAsync(role);
            Console.WriteLine($"[Reaction role] Added {role.Name} to {user.Username}");
        }

        await loadedMessage.RemoveReactionAsync(reaction.Emote, user.Id);
    }

    public async Task ReactionRemoved(Cacheable<IUserMessage, ulong> message, SocketReaction reaction)
    {
        if (reaction.UserId != 898251551183896586)
        {
            return;
        }

        if (ReactionRoles.Any(rr => rr.Emote.Equals(reaction.Emote) && rr.MessageId == message.Id))
        {
            await (await message.GetOrDownloadAsync()).AddReactionAsync(reaction.Emote,
                new RequestOptions {RetryMode = RetryMode.AlwaysRetry});
        }
    }
}