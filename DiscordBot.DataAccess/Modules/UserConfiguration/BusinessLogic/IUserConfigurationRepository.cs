using System.Threading.Tasks;

namespace DiscordBot.DataAccess.Modules.UserConfiguration.BusinessLogic;

internal interface IUserConfigurationRepository
{
    Task<string> RetrieveConfiguredLanguageAsync(string userId);
    Task SaveAsync(string userId, string language);
}