using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Contract.AutoMod;

namespace DiscordBot.DataAccess.Modules.AutoMod.BusinessLogic;

internal class AutoModBusinessLogic : IAutoModBusinessLogic
{
    private readonly IAutoModRepository _repository;

    public AutoModBusinessLogic(IAutoModRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<ulong>> GetGuildIdsWithModuleEnabled(string ruleIdentifier)
    {
        var ids = await _repository.GetGuildIdsWithModuleEnabled(ruleIdentifier);
        return ids.Select(ulong.Parse).ToArray();
    }

    public async Task<IReadOnlyList<KeyValuePair<string, string>>> GetConfigurationsForGuildAndRule(ulong guildId,
        string ruleIdentifier)
    {
        return await _repository.GetConfigurationsForGuildAndRule(guildId, ruleIdentifier);
    }
}