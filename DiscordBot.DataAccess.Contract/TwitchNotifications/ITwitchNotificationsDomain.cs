using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.DataAccess.Contract.TwitchNotifications;

public interface ITwitchNotificationsDomain
{
    Task<bool> IsStreamerInGuildAlreadyRegisteredAsync(string username, ulong guildId);
    Task SaveRegistrationAsync(TwitchNotificationRegistration registration);
    Task DeleteRegistrationAsync(string username, ulong guildId);
    Task<IEnumerable<TwitchNotificationRegistration>> RetrieveAllRegistrationsAsync();
    Task<IEnumerable<TwitchNotificationRegistration>> RetrieveAllRegistrationsForGuildAsync(ulong guildId);
}