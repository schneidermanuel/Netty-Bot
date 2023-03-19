using System.Threading.Tasks;

namespace DiscordBot.DataAccess.Modules.UserConfiguration.Domain;

internal interface IUserConfigurationRepository
{
    Task<string> RetrieveConfiguredLanguageAsync(string userId);
    Task SaveAsync(string userId, string language);
}