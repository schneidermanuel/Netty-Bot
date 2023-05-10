using System.Threading.Tasks;

namespace DiscordBot.DataAccess.Contract.MkCalculator;

public interface IMarioKartWarCacheDomain
{
    Task<MarioKartWarRegistry> RetrieveCachedRegistryAsync(ulong guildId);
    Task SaveTeamsAsync(MarioKartWarRegistry marioKartWarRegistry, ulong guildID);
}