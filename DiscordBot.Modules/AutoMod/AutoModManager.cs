using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using DiscordBot.DataAccess.Contract.AutoMod;
using DiscordBot.DataAccess.Contract.AutoMod.Violation;
using DiscordBot.Modules.AutoMod.Rules;

namespace DiscordBot.Modules.AutoMod;

internal class AutoModManager
{
    private readonly IAutoModBusinessLogic _businessLogic;
    private readonly IEnumerable<IGuildAutoModRule> _rules;

    public AutoModManager(IAutoModBusinessLogic businessLogic, IEnumerable<IGuildAutoModRule> rules)
    {
        _businessLogic = businessLogic;
        _rules = rules;
    }

    public async Task InitializeAsync()
    {
        foreach (var rule in _rules)
        {
            await rule.InitializeAsync();
        }
    }

    public async Task<IRuleViolationAction> ProcessMessage(ICommandContext context)
    {
        var rulesToTest = _rules.Where(rule => rule.IsEnabledInGuild(context.Guild.Id));
        var violation = rulesToTest
            .Select(rule => rule.ExecuteRule(context))
            .MaxBy(rule => rule.Priority);
        return violation;
    }

    public bool ExistsRule(string module)
    {
        return _rules.Any(rule => rule.RuleIdentifier == module);
    }

    public bool IsRuleEnabledForGuild(string module, ulong guildId)
    {
        if (!ExistsRule(module))
        {
            return false;
        }

        var rule = _rules.Single(rule => rule.RuleIdentifier == module);
        return rule.IsEnabledInGuild(guildId);
    }

    public async Task EnableRuleAsync(string module, ulong guildId)
    {
        if (!ExistsRule(module))
        {
            return;
        }

        var rule = _rules.Single(rule => rule.RuleIdentifier == module);
        rule.RegisterGuild(guildId);
        await _businessLogic.SetEnabled(module, guildId, true);
    }

    public async Task DisableRuleAsync(string module, ulong guildId)
    {
        if (!ExistsRule(module))
        {
            return;
        }

        var rule = _rules.Single(rule => rule.RuleIdentifier == module);
        rule.UnregisterGuild(guildId);
        await _businessLogic.SetEnabled(module, guildId, false);
    }

    public ConfigurationValueType GetValueTypeForRuleAndKey(string ruleKey, string key)
    {
        if (!ExistsRule(ruleKey))
        {
            return ConfigurationValueType.Unavailable;
        }

        var rule = _rules.Single(rule => rule.RuleIdentifier == ruleKey);
        var valueType = rule.GetValueTypeOfKey(key);
        return valueType;
    }

    public async Task SetValue(string module, string key, string value, ulong guildId)
    {
        if (!ExistsRule(module))
        {
            return;
        }

        var rule = _rules.Single(rule => rule.RuleIdentifier == module);
        rule.SetValue(guildId, key, value);
        await _businessLogic.SetValue(module, guildId, key, value);
    }

    public Dictionary<string, bool> GetModules(ulong guildId)
    {
        return _rules.ToDictionary(rule => rule.RuleIdentifier, rule => rule.IsEnabledInGuild(guildId));
    }

    public Dictionary<string, string> GetAvailableConfigs(string module)
    {
        var rule = _rules.SingleOrDefault(rule => rule.RuleIdentifier == module);
        return rule?.GetConfigurations();
    }

    public string GetConfigValue(string module, ulong guildId, string key)
    {
        var rule = _rules.SingleOrDefault(rule => rule.RuleIdentifier == module);
        return rule?.GetConfig(guildId, key);

    }
}