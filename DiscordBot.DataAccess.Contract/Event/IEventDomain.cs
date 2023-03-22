using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.DataAccess.Contract.Event;

public interface IEventDomain
{
    Task SaveAsync(Event eventDto);
    Task<IReadOnlyCollection<Event>> GetAllCurrentEventsAsync();
}