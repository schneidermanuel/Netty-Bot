using System.Collections.Generic;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Modules.ZenQuote.BusinessLogic;

namespace DiscordBot.DataAccess.Modules.ZenQuote.Repository;

public interface IZenQuoteRepository
{
    Task<IEnumerable<ZenQuoteRegistrationData>> LoadAllRegistrations();
    Task<string> RetrieveQuoteOfTheDayAsync();
    Task SaveRegistrationAsync(ZenQuoteRegistrationData registration);
    Task RemoveRegistrationAsync(long registrationId);
}