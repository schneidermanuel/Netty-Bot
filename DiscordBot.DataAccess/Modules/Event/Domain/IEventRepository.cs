using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.DataAccess.Modules.Event.Domain;

internal interface IEventRepository
{
    Task SaveAsync(Contract.Event.Event eventDto);
    Task<IReadOnlyCollection<Contract.Event.Event>> GetAllCurrentEventsAsync();
}