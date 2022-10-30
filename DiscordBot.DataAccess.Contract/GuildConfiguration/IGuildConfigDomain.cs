using System.Threading.Tasks;

namespace DiscordBot.DataAccess.Contract.GuildConfiguration;

public interface IGuildConfigDomain
{
    Task<char> GetPrefixAsync(ulong guidlId);
    Task SavePrefixAsync(ulong guildId, char prefix);
}