using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Entities;
using DiscordBot.DataAccess.Modules.YoutubeNotifications.Domain;
using DiscordBot.DataAccess.NHibernate;
using NHibernate.Linq;

namespace DiscordBot.DataAccess.Modules.YoutubeNotifications.Repository;

internal class YoutubeNotificationRepository : IYoutubeNotificationRepository
{
    private readonly ISessionProvider _provider;

    public YoutubeNotificationRepository(ISessionProvider provider)
    {
        _provider = provider;
    }

    public async Task<bool> IsStreamerInGuildAlreadyRegisteredAsync(string youtubeChannelId, ulong guildId)
    {
        using (var session = _provider.OpenSession())
        {
            var query = session.Query<YoutubeNotificationRegistrationEntity>()
                .Where(entity => entity.YoutubeChannelId == youtubeChannelId && entity.GuildId == guildId.ToString());
            return await query.AnyAsync();
        }
    }

    public async Task<long> SaveRegistrationAsync(YoutubeNotificationData data)
    {
        using (var session = _provider.OpenSession())
        {
            var entity = new YoutubeNotificationRegistrationEntity
            {
                Id = data.Id,
                Message = data.Message,
                ChannelId = data.ChannelId,
                GuildId = data.GuildId,
                YoutubeChannelId = data.YoutubeChannelId
            };
            await session.SaveOrUpdateAsync(entity);
            await session.FlushAsync();
            return entity.Id;
        }
    }

    public async Task DeleteRegistrationAsync(string youtubeChannelId, ulong guildId)
    {
        using (var session = _provider.OpenSession())
        {
            var query = session.Query<YoutubeNotificationRegistrationEntity>()
                .Where(entity => entity.YoutubeChannelId == youtubeChannelId && entity.GuildId == guildId.ToString());
            foreach (var entity in query)
            {
                await session.DeleteAsync(entity);
            }

            await session.FlushAsync();
            await session.FlushAsync();
        }
    }

    public async Task<IEnumerable<YoutubeNotificationData>> RetrieveAllRegistrationsAsync()
    {
        using (var session = _provider.OpenSession())
        {
            var query = session.Query<YoutubeNotificationRegistrationEntity>();
            return (await query.ToListAsync()).Select(MapToData);
        }
    }

    public async Task<IEnumerable<YoutubeNotificationData>> RetrieveRegistrationsByGuildIdAsync(string guildId)
    {
        using (var session = _provider.OpenSession())
        {
            var query = session.Query<YoutubeNotificationRegistrationEntity>()
                .Where(entity => entity.GuildId == guildId);
            return (await query.ToListAsync()).Select(MapToData);
        }
    }

    private YoutubeNotificationData MapToData(YoutubeNotificationRegistrationEntity entity)
    {
        return new YoutubeNotificationData(entity.Id, entity.GuildId, entity.ChannelId, entity.Message,
            entity.YoutubeChannelId);
    }
}