using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Contract.AutoMod;
using DiscordBot.DataAccess.Contract.AutoMod.Violation;

namespace DiscordBot.DataAccess.Modules.AutoMod.Domain;

internal class AutoModDomain : IAutoModDomain
{
    private readonly IAutoModRepository _repository;
    private const string IsEnabledKey = "IS_ENABLED";
    private const string ViolateActionKey = "VIOLATE_ACTION";

    public AutoModDomain(IAutoModRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<ulong>> GetGuildIdsWithModuleEnabled(string ruleIdentifier)
    {
        var ids = await _repository.GetGuildIdsWithModuleValue(ruleIdentifier, IsEnabledKey, MapBool(true));
        return ids.Select(ulong.Parse).ToArray();
    }

    public async Task<IReadOnlyList<KeyValuePair<string, string>>> GetConfigurationsForGuildAndRule(ulong guildId,
        string ruleIdentifier)
    {
        return await _repository.GetConfigurationsForGuildAndRule(guildId, ruleIdentifier);
    }

    public async Task SetEnabled(string module, ulong guildId, bool enabled)
    {
        await _repository.SetValue(module, guildId, IsEnabledKey, MapBool(enabled));
        if (enabled)
        {
            var action = await _repository.GetValueAsync(module, guildId, ViolateActionKey);
            if (action == ValidationHelper.DoNothingKey || string.IsNullOrEmpty(action))
            {
                await _repository.SetValue(module, guildId, ViolateActionKey, ValidationHelper.DeleteMessageKey);
            }
        }
    }

    public async Task SetValue(string module, ulong guildId, string key, string value)
    {
        await _repository.SetValue(module, guildId, key, value);
    }

    public async Task<IReadOnlyCollection<AutoModRule>> GetAllConfigsForGuildAsync(ulong guildId)
    {
        var ruleDatas = await _repository.GetAllConfigsForGuildAsync(guildId.ToString());
        return ruleDatas
            .GroupBy(data => data.RuleKey)
            .Select(group =>
                new AutoModRule
                {
                    RuleKey = group.Key,
                    Configs = group.Select(data =>
                        new AutoModConfig
                        {
                            Key = data.ConfigKey,
                            Value = data.Value
                        }).ToArray()
                })
            .ToArray();
    }

    private string MapBool(bool value)
    {
        return value ? "TRUE" : "FALSE";
    }
}