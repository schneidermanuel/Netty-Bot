using System.Threading.Tasks;
using DiscordBot.DataAccess.Contract;
using DiscordBot.DataAccess.Contract.UserConfiguration;

namespace DiscordBot.DataAccess;

public class ModuleDataAccess : IModuleDataAccess
{
    private readonly IUserConfigurationDomain _configurationDomain;

    public ModuleDataAccess(IUserConfigurationDomain configurationDomain)
    {
        _configurationDomain = configurationDomain;
    }

    public async Task<bool> IsModuleEnabledForGuild(ulong guildId, string moduleUniqueKey)
    {
        await Task.CompletedTask;
        return true;
    }

    public async Task<string> GetUserLanguageAsync(ulong userId)
    {
        return await _configurationDomain.GetPreferedLanguageAsync(userId);
    }
}