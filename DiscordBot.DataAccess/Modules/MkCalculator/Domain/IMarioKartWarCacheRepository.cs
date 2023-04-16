using System.Threading.Tasks;
using DiscordBot.DataAccess.Contract.MkCalculator;

namespace DiscordBot.DataAccess.Modules.MkCalculator.Domain;

internal interface IMarioKartWarCacheRepository
{
    Task<MarioKartWarRegistry> RetrieveCachedRegistryAsync(string guildId);
}