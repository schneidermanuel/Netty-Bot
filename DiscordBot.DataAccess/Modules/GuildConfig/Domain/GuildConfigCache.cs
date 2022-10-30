using System.Collections.Generic;

namespace DiscordBot.DataAccess.Modules.GuildConfig.Domain;

internal class GuildConfigCache
{
    private Dictionary<ulong, char> _chache;

    public GuildConfigCache()
    {
        _chache = new Dictionary<ulong, char>();
    }

    public char? GetCache(ulong guildId)
    {
        if (!_chache.ContainsKey(guildId))
        {
            return null;
        }

        return _chache[guildId];
    }

    public void Store(ulong guildId, char prefix)
    {
        if (_chache.ContainsKey(guildId))
        {
            _chache[guildId] = prefix;
            return;
        }

        _chache.Add(guildId, prefix);
    }
}