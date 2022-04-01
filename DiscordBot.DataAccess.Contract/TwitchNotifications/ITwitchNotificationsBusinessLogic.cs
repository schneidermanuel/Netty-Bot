using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.DataAccess.Contract.TwitchNotifications;

public interface ITwitchNotificationsBusinessLogic
{
    Task<bool> IsStreamerInGuildAlreadyRegisteredAsync(string username, ulong guildId);
    Task SaveRegistrationAsync(TwitchNotificationRegistration registration);
    Task DeleteRegistrationAsync(string username, ulong guildId);
    Task<IEnumerable<TwitchNotificationRegistration>> RetrieveAllRegistrationsAsync();
}