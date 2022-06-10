using System.Threading.Tasks;
using DiscordBot.DataAccess.Contract;
using DiscordBot.DataAccess.Contract.GuildConfiguration;
using DiscordBot.DataAccess.Contract.UserConfiguration;

namespace DiscordBot.DataAccess;

public class ModuleDataAccess : IModuleDataAccess
{
    private readonly IUserConfigurationBusinessLogic _configurationBusinessLogic;
    private readonly IGuildConfigBusinessLogic _guildConfigBusinessLogic;

    public ModuleDataAccess(IUserConfigurationBusinessLogic configurationBusinessLogic,
        IGuildConfigBusinessLogic guildConfigBusinessLogic)
    {
        _configurationBusinessLogic = configurationBusinessLogic;
        _guildConfigBusinessLogic = guildConfigBusinessLogic;
    }

    public async Task<bool> IsModuleEnabledForGuild(ulong guildId, string moduleUniqueKey)
    {
        await Task.CompletedTask;
        return true;
    }

    public async Task<char> GetServerPrefixAsync(ulong guildId)
    {
        return await _guildConfigBusinessLogic.GetPrefixAsync(guildId);
    }

    public async Task<string> GetUserLanguageAsync(ulong userId)
    {
        return await _configurationBusinessLogic.GetPreferedLanguageAsync(userId);
    }

    public async Task SetGuildPrefixAsync(ulong guildId, char prefix)
    {
        await _guildConfigBusinessLogic.SavePrefixAsync(guildId, prefix);
    }
}