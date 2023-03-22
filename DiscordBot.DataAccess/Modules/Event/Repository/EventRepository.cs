using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Entities;
using DiscordBot.DataAccess.Modules.Event.Domain;
using DiscordBot.DataAccess.NHibernate;
using NHibernate.Action;
using NHibernate.Linq;

namespace DiscordBot.DataAccess.Modules.Event.Repository;

internal class EventRepository : IEventRepository
{
    private readonly ISessionProvider _sessionProvider;

    public EventRepository(ISessionProvider sessionProvider)
    {
        _sessionProvider = sessionProvider;
    }

    public async Task SaveAsync(Contract.Event.Event eventDto)
    {
        using (var session = _sessionProvider.OpenSession())
        {
            var entity = new EventEntity
            {
                AutodeleteDate = eventDto.AutoDeleteDate,
                GuildId = eventDto.GuildId.ToString(),
                ChannelId = eventDto.ChannelId.ToString(),
                MessageId = eventDto.MessageId.ToString(),
                MaxUsers = eventDto.MaxUsers,
                RoleId = eventDto.RoleId?.ToString()
            };
            await session.SaveAsync(entity);
            await session.FlushAsync();
        }
    }

    public async Task<IReadOnlyCollection<Contract.Event.Event>> GetAllCurrentEventsAsync()
    {
        using (var session = _sessionProvider.OpenSession())
        {
            var entities = await session.Query<EventEntity>().ToListAsync();
            var dtos = entities.Select(entity => new Contract.Event.Event
            {
                ChannelId = ulong.Parse(entity.ChannelId),
                MaxUsers = entity.MaxUsers,
                RoleId = entity.RoleId != null ? ulong.Parse(entity.RoleId) : null,
                GuildId = ulong.Parse(entity.GuildId),
                MessageId = ulong.Parse(entity.MessageId),
                AutoDeleteDate = entity.AutodeleteDate,
                EventId = entity.Id
            });
            return dtos.ToArray();
        }
    }
}