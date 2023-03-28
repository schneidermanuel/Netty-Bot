using System;
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

    public async Task<long> SaveAsync(Contract.Event.Event eventDto)
    {
        using (var session = _sessionProvider.OpenSession())
        {
            var entity = new EventEntity
            {
                AutodeleteDate = eventDto.AutoDeleteDate,
                GuildId = eventDto.GuildId.ToString(),
                MaxUsers = eventDto.MaxUsers,
                RoleId = eventDto.RoleId?.ToString(),
                OwnerUserId = eventDto.OwnerUserId.ToString()
            };
            await session.SaveAsync(entity);
            await session.FlushAsync();
            return entity.Id;
        }
    }
    
    public async Task<Contract.Event.Event> GetEventByIdAsync(long eventId)
    {
        using (var session = _sessionProvider.OpenSession())
        {
            var entity = await session.LoadAsync<EventEntity>(eventId);
            return new Contract.Event.Event
            {
                MaxUsers = entity.MaxUsers,
                RoleId = entity.RoleId != null ? ulong.Parse(entity.RoleId) : null,
                GuildId = ulong.Parse(entity.GuildId),
                AutoDeleteDate = entity.AutodeleteDate,
                EventId = entity.Id,
                OwnerUserId = ulong.Parse(entity.OwnerUserId)
            };
        }
    }
}