using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using DiscordBot.DataAccess.Contract.AutoMod;
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

    public async Task ProcessMessage(ICommandContext context)
    {
        var rulesToTest = _rules.Where(rule => rule.IsEnabledInGuild(context.Guild.Id));
        var violation = rulesToTest
            .Select(rule => rule.ExecuteRule(context))
            .MaxBy(rule => rule.Priority);
        if (violation != null)
        {
            await violation.Execute(context);
        }
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
}