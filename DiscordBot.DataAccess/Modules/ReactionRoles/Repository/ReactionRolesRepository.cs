using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using DiscordBot.DataAccess.Entities;
using DiscordBot.DataAccess.Modules.ReactionRoles.BusinessLogic;
using DiscordBot.DataAccess.NHibernate;
using NHibernate.Linq;

namespace DiscordBot.DataAccess.Modules.ReactionRoles.Repository;

internal class ReactionRolesRepository : IReactionRolesRepository
{
    private readonly ISessionProvider _provider;

    public ReactionRolesRepository(ISessionProvider provider)
    {
        _provider = provider;
    }

    public async Task<IEnumerable<ReactionRoleData>> RetrieveAllReactionRoleDatasAsync()
    {
        using (var session = _provider.OpenSession())
        {
            var roles = await session.Query<ReactionRoleEntity>().ToListAsync();
            session.Close();
            return roles.Select(MapToData);
        }
    }

    public async Task SaveReactionRoleAsync(ReactionRoleData data)
    {
        using (var session = _provider.OpenSession())
        {
            var entity = new ReactionRoleEntity
            {
                ChannelId = data.ChannelId,
                EmojiId = data.EmojiId,
                GuildId = data.GuildId,
                MessageId = data.MessageId,
                RoleId = data.RoleId,
                ReactionRoleId = data.ReactionRoleId,
                IsEmoji = data.IsEmoji
            };
            await session.SaveOrUpdateAsync(entity);
            await session.FlushAsync();
        }
    }

    public async Task DeleteReactionRoleAsync(long reactionRoleId)
    {
        using (var session = _provider.OpenSession())
        {
            var entity = await session.GetAsync<ReactionRoleEntity>(reactionRoleId);
            await session.DeleteAsync(entity);
            await session.FlushAsync();
        }
    }

    public async Task<bool> CanAddReactionRoleAsync(string messageId, IEmote emote)
    {
        using (var session = _provider.OpenSession())
        {
            var query = session.Query<ReactionRoleEntity>()
                .Where(entity => entity.MessageId == messageId &&
                                 entity.EmojiId == emote.ToString());
            return !await query.AnyAsync();
        }
    }

    private ReactionRoleData MapToData(ReactionRoleEntity entity)
    {
        return new ReactionRoleData(entity.ReactionRoleId, entity.GuildId, entity.ChannelId, entity.MessageId,
            entity.EmojiId, entity.RoleId, entity.IsEmoji);
    }
}