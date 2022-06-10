using System.Threading.Tasks;

namespace DiscordBot.DataAccess.Modules.GuildConfig.BusinessLogic;

internal interface IGuildConfigRepository
{
    Task<char> GetPrefixAsync(string guidlId);
    Task SavePrefixAsync(string guildId, string prefix);
}