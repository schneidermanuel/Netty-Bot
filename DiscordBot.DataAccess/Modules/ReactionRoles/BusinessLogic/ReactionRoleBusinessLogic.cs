using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using DiscordBot.DataAccess.Contract.ReactionRoles;
using DiscordBot.DataAccess.Modules.ReactionRoles.Repository;

namespace DiscordBot.DataAccess.Modules.ReactionRoles.BusinessLogic;

public class ReactionRoleBusinessLogic : IReactionRoleBusinessLogic
{
    private readonly IReactionRolesRepository _repository;

    public ReactionRoleBusinessLogic(IReactionRolesRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ReactionRole>> RetrieveAllReactionRoleDatasAsync()
    {
        var data = await _repository.RetrieveAllReactionRoleDatasAsync();
        return data.Select(x => new ReactionRole
        {
            Id = x.ReactionRoleId,
            ChannelId = x.ChannelId,
            GuildId = x.GuildId,
            MessageId = x.MessageId,
            RoleId = x.RoleId,
            Emote = x.IsEmoji ? new Emoji(x.EmojiId) : Emote.Parse(x.EmojiId)
        });
    }

    public async Task SaveReactionRoleAsync(ReactionRole reactionRole)
    {
        var data = new ReactionRoleData
        {
            ChannelId = reactionRole.ChannelId,
            EmojiId = reactionRole.Emote.ToString(),
            GuildId = reactionRole.GuildId,
            MessageId = reactionRole.MessageId,
            RoleId = reactionRole.RoleId,
            ReactionRoleId = reactionRole.Id,
            IsEmoji = reactionRole.Emote is Emoji
        };
        await _repository.SaveReactionRoleAsync(data);
    }

    public async Task DeleteReactionRoleAsync(long reactionRoleId)
    {
        await _repository.DeleteReactionRoleAsync(reactionRoleId);
    }
}