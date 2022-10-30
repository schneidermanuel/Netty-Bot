using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Contract.YoutubeNotification;

namespace DiscordBot.DataAccess.Modules.YoutubeNotifications.Domain;

internal class YoutubeNotificationDomain : IYoutubeNotificationDomain
{
    private readonly IYoutubeNotificationRepository _repository;

    public YoutubeNotificationDomain(IYoutubeNotificationRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> IsStreamerInGuildAlreadyRegisteredAsync(string youtubeChannelId, ulong guildId)
    {
        return await _repository.IsStreamerInGuildAlreadyRegisteredAsync(youtubeChannelId, guildId);
    }

    public async Task<long> SaveRegistrationAsync(YoutubeNotificationRegistration registration)
    {
        var data = new YoutubeNotificationData(registration.RegistrationId, registration.GuildId,
            registration.ChannelId, registration.Message, registration.YoutubeChannelId);
        return await _repository.SaveRegistrationAsync(data);
    }

    public async Task DeleteRegistrationAsync(string youtubeChannelId, ulong guildId)
    {
        await _repository.DeleteRegistrationAsync(youtubeChannelId, guildId);
    }

    public async Task<IEnumerable<YoutubeNotificationRegistration>> RetrieveAllRegistrationsAsync()
    {
        var datas = await _repository.RetrieveAllRegistrationsAsync();
        return datas.Select(MapToDto);
    }

    private YoutubeNotificationRegistration MapToDto(YoutubeNotificationData data)
    {
        return new YoutubeNotificationRegistration
        {
            Message = data.Message,
            ChannelId = ulong.Parse(data.ChannelId),
            GuildId = ulong.Parse(data.GuildId),
            RegistrationId = data.Id,
            YoutubeChannelId = data.YoutubeChannelId
        };
    }
}