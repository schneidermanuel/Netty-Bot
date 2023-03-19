using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.DataAccess.Modules.YoutubeNotifications.Domain;

internal interface IYoutubeNotificationRepository
{
    Task<bool> IsStreamerInGuildAlreadyRegisteredAsync(string youtubeChannelId, ulong guildId);
    Task<long> SaveRegistrationAsync(YoutubeNotificationData data);
    Task DeleteRegistrationAsync(string youtubeChannelId, ulong guildId);
    Task<IEnumerable<YoutubeNotificationData>> RetrieveAllRegistrationsAsync();
    Task<IEnumerable<YoutubeNotificationData>> RetrieveRegistrationsByGuildIdAsync(string guildId);
}