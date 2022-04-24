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
}