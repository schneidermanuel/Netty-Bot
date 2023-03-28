using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.DataAccess.Contract.Event;

public interface IEventDomain
{
    Task<long> SaveAsync(Event eventDto);
    Task<Event> GetEventByIdAsync(long eventId);
}