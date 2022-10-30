using System.Collections.Generic;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Modules.ZenQuote.Domain;

namespace DiscordBot.DataAccess.Modules.ZenQuote.Repository;

internal interface IZenQuoteRepository
{
    Task<IEnumerable<ZenQuoteRegistrationData>> LoadAllRegistrations();
    Task<string> RetrieveQuoteOfTheDayAsync();
    Task SaveRegistrationAsync(ZenQuoteRegistrationData registration);
    Task RemoveRegistrationAsync(long registrationId);
}