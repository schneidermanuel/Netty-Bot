using System.Threading.Tasks;
using DiscordBot.DataAccess.Contract.UserConfiguration;

namespace DiscordBot.DataAccess.Modules.UserConfiguration.Domain;

internal class UserConfigurationDomain : IUserConfigurationDomain
{
    private readonly IUserConfigurationRepository _repository;
    private readonly UserConfigurationCache _cache;

    public UserConfigurationDomain(IUserConfigurationRepository repository, UserConfigurationCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<string> GetPreferedLanguageAsync(ulong userId)
    {
        var cachedValue = _cache.GetCachedValue(userId);
        if (!string.IsNullOrEmpty(cachedValue))
        {
            return cachedValue;
        }

        var language = await _repository.RetrieveConfiguredLanguageAsync(userId.ToString());
        if (string.IsNullOrEmpty(language))
        {
            return string.Empty;
        }

        _cache.Store(userId, language);
        return language;
    }

    public async Task SaveLanguageAsync(ulong userId, string language)
    {
        _cache.Store(userId, language);
        await _repository.SaveAsync(userId.ToString(), language);
    }
}