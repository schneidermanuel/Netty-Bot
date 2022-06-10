using System.Threading.Tasks;
using DiscordBot.DataAccess.Contract;
using DiscordBot.DataAccess.Contract.UserConfiguration;
using DiscordBot.DataAccess.Modules.UserConfiguration.BusinessLogic;

namespace DiscordBot.DataAccess;

public class ModuleDataAccess : IModuleDataAccess
{
    private readonly IUserConfigurationBusinessLogic _configurationBusinessLogic;

    public ModuleDataAccess(IUserConfigurationBusinessLogic configurationBusinessLogic)
    {
        _configurationBusinessLogic = configurationBusinessLogic;
    }
    public async Task<bool> IsModuleEnabledForGuild(ulong guildId, string moduleUniqueKey)
    {
        await Task.CompletedTask;
        return true;
    }

    public async Task<char> GetServerPrefixAsync(ulong guildId)
    {
        await Task.CompletedTask;
        return '!';
    }

    public async Task<string> GetUserLanguageAsync(ulong userId)
    {
        return await _configurationBusinessLogic.GetPreferedLanguageAsync(userId);
    }
}
