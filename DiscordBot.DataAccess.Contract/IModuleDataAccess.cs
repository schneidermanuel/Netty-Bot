using System.Threading.Tasks;

namespace DiscordBot.DataAccess.Contract
{
    public interface IModuleDataAccess
    {
        Task<bool> IsModuleEnabledForGuild(ulong guildId, string moduleUniqueKey);
        Task<char> GetServerPrefixAsync(ulong guildId);
        Task<string> GetUserLanguageAsync(ulong userId);
    }
}