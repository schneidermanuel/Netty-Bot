using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Contract.TwitchNotifications;

namespace DiscordBot.DataAccess.Modules.TwitchNotifications.Domain;

internal class TwitchNotificationsDomain : ITwitchNotificationsDomain
{
    private readonly ITwitchNotificationsRepository _repository;

    public TwitchNotificationsDomain(ITwitchNotificationsRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> IsStreamerInGuildAlreadyRegisteredAsync(string username, ulong guildId)
    {
        return await _repository.IsStreamerInGuildAlreadyRegisteredAsync(username, guildId);
    }

    public async Task SaveRegistrationAsync(TwitchNotificationRegistration registration)
    {
        var data = MapToData(registration);
        var id = await _repository.SaveRegistrationAsync(data);
        registration.RegistrationId = id;
    }

    public async Task DeleteRegistrationAsync(string username, ulong guildId)
    {
        await _repository.DeleteRegistrationAsync(username, guildId);
    }

    public async Task<IEnumerable<TwitchNotificationRegistration>> RetrieveAllRegistrationsAsync()
    {
        var datas = await _repository.RetrieveAllRegistrationsAsync();
        return datas.Select(MapToDto);
    }

    public async Task<IEnumerable<TwitchNotificationRegistration>> RetrieveAllRegistrationsForGuildAsync(ulong guildId)
    {
        var datas = await _repository.RetrieveAllRegistrationsForGuildAsync(guildId.ToString());
        return datas.Select(MapToDto);
    }

    private TwitchNotificationRegistration MapToDto(TwitchNotificationData data)
    {
        return new TwitchNotificationRegistration
        {
            Message = data.Message,
            Streamer = data.Streamer,
            ChannelId = ulong.Parse(data.ChannelId),
            GuildId = ulong.Parse(data.GuildId),
            RegistrationId = data.Id
        };
    }

    private TwitchNotificationData MapToData(TwitchNotificationRegistration registration)
    {
        return new TwitchNotificationData(registration.RegistrationId, registration.GuildId, registration.ChannelId,
            registration.Message, registration.Streamer);
    }
}