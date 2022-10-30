using System.Collections.Generic;

namespace DiscordBot.DataAccess.Modules.UserConfiguration.Domain;

internal class UserConfigurationCache
{
    private Dictionary<ulong, string> _languageCache;

    public UserConfigurationCache()
    {
        _languageCache = new Dictionary<ulong, string>();
    }

    public string GetCachedValue(ulong userId)
    {
        if (_languageCache.ContainsKey(userId))
        {
            return _languageCache[userId];
        }

        return null;
    }

    public void Store(ulong userId, string language)
    {
        if (_languageCache.ContainsKey(userId))
        {
            _languageCache[userId] = language;
            return;
        }

        _languageCache.Add(userId, language);
    }
}