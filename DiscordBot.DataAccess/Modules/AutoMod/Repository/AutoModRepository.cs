using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Entities;
using DiscordBot.DataAccess.Modules.AutoMod.BusinessLogic;
using DiscordBot.DataAccess.NHibernate;
using NHibernate.Linq;

namespace DiscordBot.DataAccess.Modules.AutoMod.Repository;

internal class AutoModRepository : IAutoModRepository
{
    private readonly ISessionProvider _provider;

    public AutoModRepository(ISessionProvider provider)
    {
        _provider = provider;
    }

    public async Task<IReadOnlyCollection<string>> GetGuildIdsWithModuleEnabled(string ruleIdentifier)
    {
        using (var session = _provider.OpenSession())
        {
            var query = session.Query<AutoModConfigurationEntity>()
                .Where(entity => entity.RuleKey == ruleIdentifier
                                 && entity.ConfigurationKey == "IS_ENABLED"
                                 && entity.ConfigurationValue == "TRUE")
                .Select(entity => entity.GuildId);
            return await query.ToListAsync();
        }
    }

    public async Task<IReadOnlyList<KeyValuePair<string, string>>> GetConfigurationsForGuildAndRule(ulong guildId,
        string ruleIdentifier)
    {
        using (var session = _provider.OpenSession())
        {
            var query = session.Query<AutoModConfigurationEntity>()
                .Where(entity => entity.RuleKey == ruleIdentifier
                                 && entity.GuildId == guildId.ToString())
                .Select(entity => new KeyValuePair<string, string>(entity.ConfigurationKey, entity.ConfigurationValue));
            return await query.ToListAsync();
        }
    }
}