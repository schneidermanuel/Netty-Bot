using System.Threading.Tasks;
using DiscordBot.DataAccess.Contract;

namespace DiscordBot.DataAccess;

public class ModuleDataAccess : IModuleDataAccess
{
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
}