using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Contract.ZenQuote;
using DiscordBot.DataAccess.Modules.ZenQuote.Repository;

namespace DiscordBot.DataAccess.Modules.ZenQuote.Domain;

internal class ZenQuoteDomain : IZenQuoteDomain
{
    private readonly IZenQuoteRepository _repository;

    public ZenQuoteDomain(IZenQuoteRepository repository)
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
        var data = new ZenQuoteRegistrationData(registration.Id, registration.GuildId, registration.Channelid);
        await _repository.SaveRegistrationAsync(data);
    }

    public async Task RemoveRegistrationAsync(long registrationId)
    {
        await _repository.RemoveRegistrationAsync(registrationId);
    }

    public async Task<IEnumerable<ZenQuoteRegistration>> LoadAllRegistrationsForGuildAsync(ulong guildId)
    {
        var data = await _repository.LoadAllRegistrationsForGuildAsync(guildId.ToString());
        var domains = data.Select(MapToDomain);
        return domains;
    }

    private ZenQuoteRegistration MapToDomain(ZenQuoteRegistrationData data)
    {
        return new ZenQuoteRegistration
        {
            Channelid = ulong.Parse(data.ChannelId),
            Id = data.Id,
            GuildId = ulong.Parse(data.GuildId)
        };
    }
}