using System.Threading.Tasks;
using Discord.Commands;
using DiscordBot.Modules.AutoMod.Rules.Violation;

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
}