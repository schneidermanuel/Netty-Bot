using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Entities;
using DiscordBot.DataAccess.Modules.ReactionRoles.BusinessLogic;
using DiscordBot.DataAccess.NHibernate;
using NHibernate.Linq;

namespace DiscordBot.DataAccess.Modules.ReactionRoles.Repository;

public class ReactionRolesRepository : IReactionRolesRepository
{
    private readonly ISessionFactoryProvider _provider;

    public ReactionRolesRepository(ISessionFactoryProvider provider)
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
                ChannelId = data.ChannelId.ToString(),
                EmojiId = data.EmojiId,
                GuildId = data.GuildId.ToString(),
                MessageId = data.MessageId.ToString(),
                RoleId = data.RoleId.ToString(),
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

    private ReactionRoleData MapToData(ReactionRoleEntity entity)
    {
        return new ReactionRoleData
        {
            ChannelId = ulong.Parse(entity.ChannelId),
            EmojiId = entity.EmojiId,
            GuildId = ulong.Parse(entity.GuildId),
            MessageId = ulong.Parse(entity.MessageId),
            RoleId = ulong.Parse(entity.RoleId),
            ReactionRoleId = entity.ReactionRoleId,
            IsEmoji = entity.IsEmoji
        };
    }
}