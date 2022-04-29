using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using DiscordBot.DataAccess.Contract.AutoMod.Violation;

namespace DiscordBot.Modules.AutoMod.Rules;

internal interface IGuildAutoModRule
{
    string RuleIdentifier { get; }
    IRuleViolationAction ExecuteRule(ICommandContext context);
    bool IsEnabledInGuild(ulong guildId);
    void RegisterGuild(ulong guildId);
    void UnregisterGuild(ulong guildId);
    void SetValue(ulong guildId, string key, string value);
    Task InitializeAsync();
    ConfigurationValueType GetValueTypeOfKey(string key);
    Dictionary<string, string> GetConfigurations();
    string GetValue(ulong guildId, string key);
}