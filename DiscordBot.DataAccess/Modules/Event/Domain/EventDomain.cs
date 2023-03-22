using System.Collections.Generic;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Contract.Event;

namespace DiscordBot.DataAccess.Modules.Event.Domain;

internal class EventDomain : IEventDomain
{
    private readonly IEventRepository _repository;

    public EventDomain(IEventRepository repository)
    {
        _repository = repository;
    }
    
    public async Task SaveAsync(Contract.Event.Event eventDto)
    {
        await _repository.SaveAsync(eventDto);
    }

    public async Task<IReadOnlyCollection<Contract.Event.Event>> GetAllCurrentEventsAsync()
    {
        return await _repository.GetAllCurrentEventsAsync();
    }
}