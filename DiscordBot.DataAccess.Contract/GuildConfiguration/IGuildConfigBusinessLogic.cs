using System.Threading.Tasks;

namespace DiscordBot.DataAccess.Contract.GuildConfiguration;

public interface IGuildConfigBusinessLogic
{
    Task<char> GetPrefixAsync(ulong guidlId);
    Task SavePrefixAsync(ulong guildId, char prefix);
}