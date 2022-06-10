using System.Threading.Tasks;
using DiscordBot.DataAccess.Contract.GuildConfiguration;

namespace DiscordBot.DataAccess.Modules.GuildConfig.BusinessLogic;

internal class GuildConfigBusinessLogic : IGuildConfigBusinessLogic
{
    private readonly IGuildConfigRepository _repository;
    private readonly GuildConfigCache _cache;

    public GuildConfigBusinessLogic(IGuildConfigRepository repository, GuildConfigCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<char> GetPrefixAsync(ulong guidlId)
    {
        var cache = _cache.GetCache(guidlId);
        if (cache.HasValue)
        {
            return cache.Value;
        }

        var prefix = await _repository.GetPrefixAsync(guidlId.ToString());
        _cache.Store(guidlId, prefix);
        return prefix;
    }

    public async Task SavePrefixAsync(ulong guildId, char prefix)
    {
        _cache.Store(guildId, prefix);
        await _repository.SavePrefixAsync(guildId.ToString(), prefix.ToString());
    }
}