using System.Collections.Generic;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Contract.TwitterRegistration;

namespace DiscordBot.DataAccess.Modules.TwitterRegistration.Domain;

internal interface ITwitterRegistrationRepository
{
    Task<bool> IsAccountRegisteredOnChannelAsync(string guildId, string channelId, string username);
    Task RegisterTwitterAsync(TwitterRegistrationData data);
    Task<IReadOnlyCollection<TwitterRegistrationData>> RetrieveAllTwitterRegistrationsAsync();
    Task UnregisterTwitterAsync(string guildId, string channelId, string username);
    Task<IReadOnlyCollection<TwitterRegistrationData>> RetrieveAllRegistartionsForGuildAsync(string guildId);
}