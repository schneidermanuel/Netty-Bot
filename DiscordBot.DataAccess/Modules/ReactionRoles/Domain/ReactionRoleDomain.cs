using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using DiscordBot.DataAccess.Contract.ReactionRoles;

namespace DiscordBot.DataAccess.Modules.ReactionRoles.Domain;

internal class ReactionRoleDomain : IReactionRoleDomain
{
    private readonly IReactionRolesRepository _repository;

    public ReactionRoleDomain(IReactionRolesRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ReactionRole>> RetrieveAllReactionRoleDatasAsync()
    {
        var data = await _repository.RetrieveAllReactionRoleDatasAsync();
        return data.Select(x => new ReactionRole
        {
            Id = x.ReactionRoleId,
            ChannelId = ulong.Parse(x.ChannelId),
            GuildId = ulong.Parse(x.GuildId),
            MessageId = ulong.Parse(x.MessageId),
            RoleId = ulong.Parse(x.RoleId),
            Emote = GetEmote(x)
        });
    }

    public async Task SaveReactionRoleAsync(ReactionRole reactionRole)
    {
        var data = new ReactionRoleData(reactionRole.Id, reactionRole.GuildId, reactionRole.ChannelId,
            reactionRole.MessageId, reactionRole.Emote.ToString(), reactionRole.RoleId, reactionRole.Emote is Emoji);
        await _repository.SaveReactionRoleAsync(data);
    }

    public async Task DeleteReactionRoleAsync(long reactionRoleId)
    {
        await _repository.DeleteReactionRoleAsync(reactionRoleId);
    }

    public async Task<bool> CanAddReactionRoleAsync(ulong messageId, IEmote emote)
    {
        return await _repository.CanAddReactionRoleAsync(messageId.ToString(), emote);
    }

    public async Task<IEnumerable<ReactionRole>> RetrieveReactionRolesForGuildAsync(ulong guildId)
    {
        var data = await _repository.RetrieveAllReactionRoleForGuildAsync(guildId);
        return data.Select(x => new ReactionRole
        {
            Id = x.ReactionRoleId,
            ChannelId = ulong.Parse(x.ChannelId),
            GuildId = ulong.Parse(x.GuildId),
            MessageId = ulong.Parse(x.MessageId),
            RoleId = ulong.Parse(x.RoleId),
            Emote = GetEmote(x)
        });
    }

    private static IEmote GetEmote(ReactionRoleData data)
    {
        if (!data.IsEmoji)
        {
            return Emote.Parse(data.EmojiId);
        }

        if (data.EmojiId.StartsWith("U+"))
        {
            var charEmoji = char.ConvertFromUtf32(int.Parse(data.EmojiId[2..], NumberStyles.HexNumber));
            Console.WriteLine(data.EmojiId);
            Console.WriteLine(charEmoji);
            try
            {
                return Emoji.Parse(charEmoji);
            }
            catch (Exception)
            {
                return Emoji.Parse("⚠️");
            }
        }

        return Emoji.Parse(data.EmojiId);
    }
}