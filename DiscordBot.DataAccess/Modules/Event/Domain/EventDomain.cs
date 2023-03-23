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
    
    public async Task<long> SaveAsync(Contract.Event.Event eventDto)
    {
        return await _repository.SaveAsync(eventDto);
    }

    public async Task<IReadOnlyCollection<Contract.Event.Event>> GetAllCurrentEventsAsync()
    {
        return await _repository.GetAllCurrentEventsAsync();
    }

    public async Task<Contract.Event.Event> GetEventByIdAsync(long eventId)
    {
        return await _repository.GetEventByIdAsync(eventId);
    }
}