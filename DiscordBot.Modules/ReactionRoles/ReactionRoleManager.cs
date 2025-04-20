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
    private readonly DiscordSocketClient _client;
    private readonly IReactionRoleDomain _domain;

    public ReactionRoleManager(DiscordSocketClient client, IReactionRoleDomain domain)
    {
        _client = client;
        _domain = domain;
    }

    public List<ReactionRole> ReactionRoles;

    public async Task ReactionAdded(Cacheable<IUserMessage, ulong> message,
        SocketReaction reaction)
    {
        var reactionRoles = ReactionRoles.Where(role => role.MessageId == message.Id);
        foreach (var reactionRole in reactionRoles)
        {
            try
            {
                await ProcessReactionRoleAsync(message, reaction, reactionRole);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    private static async Task ProcessReactionRoleAsync(Cacheable<IUserMessage, ulong> message, SocketReaction reaction,
        ReactionRole reactionRole)
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
                new RequestOptions { RetryMode = RetryMode.AlwaysRetry });
        }
    }

    public async Task RefreshGuildAsync(ulong guildId, IEnumerable<ReactionRole> reactionRoles)
    {
        var rolesOfGuild = ReactionRoles.Where(r => r.GuildId == guildId).ToArray();
        foreach (var reactionRole in rolesOfGuild)
        {
            ReactionRoles.Remove(reactionRole);
        }

        foreach (var reactionRole in reactionRoles)
        {
            if (await ProcessReactionRole(reactionRole))
            {
                ReactionRoles.Add(reactionRole);
            }
        }
    }

    public async Task<bool> ProcessReactionRole(ReactionRole reactionRole)
    {
        try
        {
            var guild = _client.GetGuild(reactionRole.GuildId);
            var message =
                await ((ISocketMessageChannel)guild.GetChannel(reactionRole.ChannelId))
                    .GetMessageAsync(reactionRole.MessageId);
            var emote = reactionRole.Emote;
            await message.RemoveAllReactionsForEmoteAsync(emote);
            await Task.Delay(2000);
            await message.AddReactionAsync(emote);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await _domain.DeleteReactionRoleAsync(reactionRole.Id);
            return false;
        }
    }
}