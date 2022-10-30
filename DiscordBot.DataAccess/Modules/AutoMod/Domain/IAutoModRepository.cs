using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.DataAccess.Modules.AutoMod.Domain;

internal interface IAutoModRepository
{
    Task<IReadOnlyCollection<string>> GetGuildIdsWithModuleValue(string ruleIdentifier, string key, string value);
    Task<IReadOnlyList<KeyValuePair<string,string>>> GetConfigurationsForGuildAndRule(ulong guildId, string ruleIdentifier);
    Task SetValue(string module, ulong guildId, string key, string value);
    Task<string> GetValueAsync(string ruleKey, ulong guildId, string key);
}