using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.DataAccess.Contract.ZenQuote
{
    public interface IZenQuoteDomain
    {
        Task<IEnumerable<ZenQuoteRegistration>> LoadAllRegistrations();
        Task<string> RetrieveQuoteOfTheDayAsync();
        Task SaveRegistrationAsync(ZenQuoteRegistration registration);
        Task RemoveRegistrationAsync(long registrationId);
        Task<IEnumerable<ZenQuoteRegistration>> LoadAllRegistrationsForGuildAsync(ulong guildId);
    }
}