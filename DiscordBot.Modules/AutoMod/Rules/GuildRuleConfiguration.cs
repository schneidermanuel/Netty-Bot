using System.Collections.Generic;

namespace DiscordBot.Modules.AutoMod.Rules;

internal class GuildRuleConfiguration
{
    public Dictionary<string, string> Configs;

    public GuildRuleConfiguration()
    {
        Configs = new Dictionary<string, string>();
    }

    public void SetValue(string key, string value)
    {
        if (!Configs.ContainsKey(key))
        {
            Configs.Add(key, value);
            return;
        }

        Configs[key] = value;
    }

    public string GetValue(string key)
    {
        return Configs.ContainsKey(key) ? Configs[key] : null;
    }
}