using System.Threading.Tasks;

namespace DiscordBot.DataAccess.Contract
{
    public interface IModuleDataAccess
    {
        Task<bool> IsModuleEnabledForGuild(ulong guildId, string moduleUniqueKey);
        Task<string> GetUserLanguageAsync(ulong userId);
    }
}