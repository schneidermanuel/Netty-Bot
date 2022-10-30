using System.Threading.Tasks;
using DiscordBot.DataAccess.Contract;
using DiscordBot.DataAccess.Contract.GuildConfiguration;
using DiscordBot.DataAccess.Contract.UserConfiguration;

namespace DiscordBot.DataAccess;

public class ModuleDataAccess : IModuleDataAccess
{
    private readonly IUserConfigurationDomain _configurationDomain;
    private readonly IGuildConfigDomain _guildConfigDomain;

    public ModuleDataAccess(IUserConfigurationDomain configurationDomain,
        IGuildConfigDomain guildConfigDomain)
    {
        _configurationDomain = configurationDomain;
        _guildConfigDomain = guildConfigDomain;
    }

    public async Task<bool> IsModuleEnabledForGuild(ulong guildId, string moduleUniqueKey)
    {
        await Task.CompletedTask;
        return true;
    }

    public async Task<char> GetServerPrefixAsync(ulong guildId)
    {
        return await _guildConfigDomain.GetPrefixAsync(guildId);
    }

    public async Task<string> GetUserLanguageAsync(ulong userId)
    {
        return await _configurationDomain.GetPreferedLanguageAsync(userId);
    }

    public async Task SetGuildPrefixAsync(ulong guildId, char prefix)
    {
        await _guildConfigDomain.SavePrefixAsync(guildId, prefix);
    }
}