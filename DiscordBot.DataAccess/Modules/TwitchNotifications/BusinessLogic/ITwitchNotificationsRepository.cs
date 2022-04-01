using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.DataAccess.Modules.TwitchNotifications.BusinessLogic;

internal interface ITwitchNotificationsRepository
{
    Task<bool> IsStreamerInGuildAlreadyRegisteredAsync(string username, ulong guildId);
    Task<long> SaveRegistrationAsync(TwitchNotificationData data);
    Task DeleteRegistrationAsync(string username, ulong guildId);
    Task<IEnumerable<TwitchNotificationData>> RetrieveAllRegistrationsAsync();
}