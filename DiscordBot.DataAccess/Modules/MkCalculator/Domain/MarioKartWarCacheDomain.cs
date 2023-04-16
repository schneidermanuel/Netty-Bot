using System.Threading.Tasks;
using DiscordBot.DataAccess.Contract.MkCalculator;

namespace DiscordBot.DataAccess.Modules.MkCalculator.Domain;

internal class MarioKartWarCacheDomain : IMarioKartWarCacheDomain
{
    private readonly IMarioKartWarCacheRepository _repository;

    public MarioKartWarCacheDomain(IMarioKartWarCacheRepository repository)
    {
        _repository = repository;
    }
    public async Task<MarioKartWarRegistry> RetrieveCachedRegistryAsync(ulong guildId)
    {
        return await _repository.RetrieveCachedRegistryAsync(guildId.ToString());
    }
}