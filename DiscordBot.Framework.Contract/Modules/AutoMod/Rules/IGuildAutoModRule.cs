using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using DiscordBot.DataAccess.Contract.AutoMod.Violation;

namespace DiscordBot.Framework.Contract.Modules.AutoMod.Rules;

public interface IGuildAutoModRule
{
    string RuleIdentifier { get; }
    IRuleViolationAction ExecuteRule(ICommandContext context);
    bool IsEnabledInGuild(ulong guildId);
    void RegisterGuild(ulong guildId);
    void UnregisterGuild(ulong guildId);
    void SetValue(ulong guildId, string key, string value);
    Task InitializeAsync();
    ConfigurationValueType GetValueTypeOfKey(string key);
    Dictionary<string, ConfigurationValueType> GetConfigurations();
    string GetConfig(ulong guildId, string key);
    void UnsetAllValues(ulong guildId);
}