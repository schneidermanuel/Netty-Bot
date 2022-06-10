using System.Threading.Tasks;

namespace DiscordBot.DataAccess.Contract.UserConfiguration;

public interface IUserConfigurationBusinessLogic
{
    Task<string> GetPreferedLanguageAsync(ulong userId);
    Task SaveLanguageAsync(ulong userId, string language);
}