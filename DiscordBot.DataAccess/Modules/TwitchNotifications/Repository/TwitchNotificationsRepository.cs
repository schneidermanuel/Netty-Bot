using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Entities;
using DiscordBot.DataAccess.Hibernate;
using DiscordBot.DataAccess.Modules.TwitchNotifications.Domain;
using NHibernate.Linq;

namespace DiscordBot.DataAccess.Modules.TwitchNotifications.Repository;

internal class TwitchNotificationsRepository : ITwitchNotificationsRepository
{
    private readonly ISessionProvider _provider;

    public TwitchNotificationsRepository(ISessionProvider provider)
    {
        _provider = provider;
    }

    public async Task<bool> IsStreamerInGuildAlreadyRegisteredAsync(string username, ulong guildId)
    {
        using (var session = _provider.OpenSession())
        {
            var query = session.Query<TwitchNotificationRegistrationEntity>()
                .Where(entity => entity.Streamer == username
                                 && entity.GuildId == guildId.ToString());
            return await query.AnyAsync();
        }
    }

    public async Task<long> SaveRegistrationAsync(TwitchNotificationData data)
    {
        var entity = new TwitchNotificationRegistrationEntity
        {
            Id = data.Id,
            Message = data.Message,
            Streamer = data.Streamer,
            ChannelId = data.ChannelId,
            GuildId = data.GuildId
        };
        using (var session = _provider.OpenSession())
        {
            await session.SaveAsync(entity);
            await session.FlushAsync();
            return entity.Id;
        }
    }

    public async Task DeleteRegistrationAsync(string username, ulong guildId)
    {
        using (var session = _provider.OpenSession())
        {
            var query = session.Query<TwitchNotificationRegistrationEntity>()
                .Where(entity => entity.Streamer == username
                                 && entity.GuildId == guildId.ToString());
            foreach (var entity in query)
            {
                await session.DeleteAsync(entity);
            }

            await session.FlushAsync();
        }
    }

    public async Task<IEnumerable<TwitchNotificationData>> RetrieveAllRegistrationsAsync()
    {
        using (var session = _provider.OpenSession())
        {
            var query = session.Query<TwitchNotificationRegistrationEntity>();
            var entites = await query.ToListAsync();
            return entites.Select(MapToData);
        }
    }

    public async Task<IEnumerable<TwitchNotificationData>> RetrieveAllRegistrationsForGuildAsync(string guildId)
    {
        using (var session = _provider.OpenSession())
        {
            var query = session.Query<TwitchNotificationRegistrationEntity>()
                .Where(entity => 
                    entity.GuildId == guildId);
            var entites = await query.ToListAsync();
            return entites.Select(MapToData);
        }
    }

    private TwitchNotificationData MapToData(TwitchNotificationRegistrationEntity entity)
    {
        return new TwitchNotificationData(entity.Id, entity.GuildId, entity.ChannelId, entity.Message, entity.Streamer);
    }
}