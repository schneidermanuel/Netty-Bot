using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.DataAccess.Modules.Event.Domain;

internal interface IEventRepository
{
    Task<long> SaveAsync(Contract.Event.Event eventDto);
    Task<IReadOnlyCollection<Contract.Event.Event>> GetAllCurrentEventsAsync();
    Task<Contract.Event.Event> GetEventByIdAsync(long eventId);
}