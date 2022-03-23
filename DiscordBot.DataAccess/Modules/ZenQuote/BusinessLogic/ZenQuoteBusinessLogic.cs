using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Contract.ZenQuote;
using DiscordBot.DataAccess.Modules.ZenQuote.Repository;

namespace DiscordBot.DataAccess.Modules.ZenQuote.BusinessLogic;

public class ZenQuoteBusinessLogic : IZenQuoteBusinessLogic
{
    private readonly IZenQuoteRepository _repository;

    public ZenQuoteBusinessLogic(IZenQuoteRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ZenQuoteRegistration>> LoadAllRegistrations()
    {
        var data = await _repository.LoadAllRegistrations();
        var domains = data.Select(MapToDomain);
        return domains;
    }

    public async Task<string> RetrieveQuoteOfTheDayAsync()
    {
        return await _repository.RetrieveQuoteOfTheDayAsync();
    }

    public async Task SaveRegistrationAsync(ZenQuoteRegistration registration)
    {
        var data = new ZenQuoteRegistrationData
        {
            Channelid = registration.Channelid,
            Id = registration.Id,
            GuildId = registration.GuildId
        };
        await _repository.SaveRegistrationAsync(data);
    }

    public async Task RemoveRegistrationAsync(long registrationId)
    {
        await _repository.RemoveRegistrationAsync(registrationId);
    }

    private ZenQuoteRegistration MapToDomain(ZenQuoteRegistrationData data)
    {
        return new ZenQuoteRegistration
        {
            Channelid = data.Channelid,
            Id = data.Id,
            GuildId = data.GuildId
        };
    }
}