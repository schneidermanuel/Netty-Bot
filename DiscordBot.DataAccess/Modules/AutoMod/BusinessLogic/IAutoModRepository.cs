using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.DataAccess.Modules.AutoMod.BusinessLogic;

internal interface IAutoModRepository
{
    Task<IReadOnlyCollection<string>> GetGuildIdsWithModuleEnabled(string ruleIdentifier);
    Task<IReadOnlyList<KeyValuePair<string,string>>> GetConfigurationsForGuildAndRule(ulong guildId, string ruleIdentifier);
}