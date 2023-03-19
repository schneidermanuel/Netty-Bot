using System.Threading.Tasks;

namespace DiscordBot.DataAccess.Contract.UserConfiguration;

public interface IUserConfigurationDomain
{
    Task<string> GetPreferedLanguageAsync(ulong userId);
    Task SaveLanguageAsync(ulong userId, string language);
}