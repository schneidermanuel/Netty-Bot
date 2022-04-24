using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.DataAccess.Contract.AutoMod;

public interface IAutoModBusinessLogic
{
    Task<IReadOnlyCollection<ulong>> GetGuildIdsWithModuleEnabled(string ruleIdentifier);
    Task<IReadOnlyList<KeyValuePair<string, string>>> GetConfigurationsForGuildAndRule(ulong guildId, string ruleIdentifier);
    Task SetEnabled(string module, ulong guildId, bool enabled);
}