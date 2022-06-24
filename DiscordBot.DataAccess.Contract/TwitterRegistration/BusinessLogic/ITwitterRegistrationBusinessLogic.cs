using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.DataAccess.Contract.TwitterRegistration.BusinessLogic;

public interface ITwitterRegistrationBusinessLogic
{
    Task<bool> IsAccountRegisteredOnChannelAsync(ulong guildId, ulong channelId, string username);
    Task RegisterTwitterAsync(TwitterRegistrationDto twitterRegistrationDto);
    Task<IReadOnlyCollection<TwitterRegistrationDto>> RetrieveAllRegistartionsAsync();
}