using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using DiscordBot.DataAccess.Contract.AutoMod;
using DiscordBot.DataAccess.Contract.AutoMod.Violation;
using MySqlX.XDevAPI.Common;

namespace DiscordBot.Modules.AutoMod.Rules;

internal abstract class AutoModRuleBase : IGuildAutoModRule
{
    private readonly IAutoModBusinessLogic _businessLogic;
    protected IList<ulong> Guilds { get; set; }
    protected Dictionary<ulong, GuildRuleConfiguration> Configs { get; set; }
    protected abstract Dictionary<string, ConfigurationValueType> _keys { get; }
    public abstract string RuleIdentifier { get; }


    protected AutoModRuleBase(IAutoModBusinessLogic businessLogic)
    {
        _businessLogic = businessLogic;
        Configs = new Dictionary<ulong, GuildRuleConfiguration>();
        Guilds = new List<ulong>();
    }

    public ConfigurationValueType GetValueTypeOfKey(string key)
    {
        return _keys.ContainsKey(key)
            ? _keys[key]
            : ConfigurationValueType.Unavailable;
    }

    public Dictionary<string, string> GetConfigurations()
    {
        return _keys.ToDictionary(key => key.Key, key => ValidationHelper.MapValueTypeToString(key.Value));
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
        return Guilds.Contains(guildId);
    }

    public void RegisterGuild(ulong guildId)
    {
        if (!Guilds.Contains(guildId))
        {
            Guilds.Add(guildId);
        }

        if (!Configs.ContainsKey(guildId))
        {
            var config = new GuildRuleConfiguration();
            config.SetValue(ValidationHelper.ActionKey, ValidationHelper.DeleteMessageKey);
            Configs.Add(guildId, config);
        }
    }

    public void UnregisterGuild(ulong guildId)
    {
        if (Guilds.Contains(guildId))
        {
            Guilds.Remove(guildId);
        }
    }

    public async Task InitializeAsync()
    {
        var guilds = await _businessLogic.GetGuildIdsWithModuleEnabled(RuleIdentifier);
        Guilds = guilds.ToList();
        foreach (var enabledGuild in Guilds)
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