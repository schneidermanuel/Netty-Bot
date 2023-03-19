using System.Threading.Tasks;
using DiscordBot.DataAccess.Contract.AutoMod;
using DiscordBot.Framework.Contract.Modules.AutoMod;

namespace DiscordBot.Modules.AutoMod;

internal class AutoModRefresher : IAutoModRefresher
{
    private readonly IAutoModDomain _domain;
    private readonly AutoModManager _manager;

    public AutoModRefresher(IAutoModDomain domain, AutoModManager manager)
    {
        _domain = domain;
        _manager = manager;
    }

    public async Task RefreshGuildAsync(ulong guildId)
    {
        var configs = await _domain.GetAllConfigsForGuildAsync(guildId);
        foreach (var rule in configs)
        {
            _manager.UnsetAllValues(rule.RuleKey, guildId);
            foreach (var ruleConfig in rule.Configs)
            {
                await _manager.SetValue(rule.RuleKey, ruleConfig.Key, ruleConfig.Value, guildId);
            }
        }
    }
}