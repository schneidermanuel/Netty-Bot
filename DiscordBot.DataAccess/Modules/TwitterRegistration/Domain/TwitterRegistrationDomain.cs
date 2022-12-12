using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Contract.TwitterRegistration;
using DiscordBot.DataAccess.Contract.TwitterRegistration.Domain;

namespace DiscordBot.DataAccess.Modules.TwitterRegistration.Domain;

internal class TwitterRegistrationDomain : ITwitterRegistrationDomain
{
    private readonly ITwitterRegistrationRepository _repository;

    public TwitterRegistrationDomain(ITwitterRegistrationRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> IsAccountRegisteredOnChannelAsync(ulong guildId, ulong channelId, string username)
    {
        return await _repository.IsAccountRegisteredOnChannelAsync(guildId.ToString(), channelId.ToString(), username);
    }

    public async Task RegisterTwitterAsync(TwitterRegistrationDto registrationDto)
    {
        var data = MapDtoToData(registrationDto);
        await _repository.RegisterTwitterAsync(data);
    }

    private TwitterRegistrationData MapDtoToData(TwitterRegistrationDto registrationDto)
    {
        return new TwitterRegistrationData(registrationDto.RegistrationId, registrationDto.GuildId, registrationDto.ChannelId, registrationDto.Username, registrationDto.Message, registrationDto.RuleFilter);
    }

    public async Task<IReadOnlyCollection<TwitterRegistrationDto>> RetrieveAllRegistartionsAsync()
    {
        var datas = await _repository.RetrieveAllTwitterRegistrationsAsync();
        return datas.Select(MapDataToDto).ToArray();
    }

    public async Task UnregisterTwitterAsync(ulong guildId, ulong channelId, string username)
    {
        await _repository.UnregisterTwitterAsync(guildId.ToString(), channelId.ToString(), username);
    }

    public async Task<IReadOnlyCollection<TwitterRegistrationDto>> RetrieveAllRegistartionsForGuildAsync(ulong guildId)
    {
        var datas = await _repository.RetrieveAllRegistartionsForGuildAsync(guildId.ToString());
        return datas.Select(MapDataToDto).ToArray();
    }

    private TwitterRegistrationDto MapDataToDto(TwitterRegistrationData data)
    {
        return new TwitterRegistrationDto
        {
            Username = data.TwitterUsername,
            ChannelId = ulong.Parse(data.ChannelId),
            GuildId = ulong.Parse(data.GuildId),
            Message = data.Message,
            RegistrationId = data.RegistrationId,
            RuleFilter = data.RuleFilter
        };
    }
}