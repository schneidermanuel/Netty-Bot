using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using DiscordBot.DataAccess.Contract.AutoMod;
using DiscordBot.DataAccess.Contract.AutoMod.Violation;

namespace DiscordBot.Modules.AutoMod.Rules;

internal abstract class AutoModRuleBase : IGuildAutoModRule
{
    private readonly IAutoModBusinessLogic _businessLogic;
    protected IList<ulong> _guilds { get; set; }
    protected Dictionary<ulong, GuildRuleConfiguration> Configs { get; set; }
    protected abstract Dictionary<string, ConfigurationValueType> _keys { get; }
    public abstract string RuleIdentifier { get; }


    protected AutoModRuleBase(IAutoModBusinessLogic businessLogic)
    {
        _businessLogic = businessLogic;
        Configs = new Dictionary<ulong, GuildRuleConfiguration>();
    }

    public ConfigurationValueType GetValueTypeOfKey(string key)
    {
        return _keys.ContainsKey(key)
            ? _keys[key]
            : ConfigurationValueType.Unavailable;
    }

    public void SetValue(ulong guildId, string key, string value)
    {
        if (!Configs.ContainsKey(guildId))
        {
            Configs.Add(guildId, new GuildRuleConfiguration());
        }

        Configs[guildId].SetValue(key, value);
    }

    public abstract IRuleViolationAction ExecuteRule(ICommandContext context);

    public bool IsEnabledInGuild(ulong guildId)
    {
        return _guilds.Contains(guildId);
    }

    public void RegisterGuild(ulong guildId)
    {
        if (!_guilds.Contains(guildId))
        {
            _guilds.Add(guildId);
        }
    }

    public void UnregisterGuild(ulong guildId)
    {
        if (_guilds.Contains(guildId))
        {
            _guilds.Remove(guildId);
        }
    }

    public async Task InitializeAsync()
    {
        var guilds = await _businessLogic.GetGuildIdsWithModuleEnabled(RuleIdentifier);
        _guilds = guilds.ToList();
        foreach (var enabledGuild in _guilds)
        {
            var keyValuePairs = await _businessLogic.GetConfigurationsForGuildAndRule(enabledGuild, RuleIdentifier);
            foreach (var keyValuePair in keyValuePairs)
            {
                if (!Configs.ContainsKey(enabledGuild))
                {
                    Configs.Add(enabledGuild, new GuildRuleConfiguration());
                }

                Configs[enabledGuild].SetValue(keyValuePair.Key, keyValuePair.Value);
            }
        }
    }
}