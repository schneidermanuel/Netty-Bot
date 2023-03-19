using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Entities;
using DiscordBot.DataAccess.Modules.AutoMod.Domain;
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

    public async Task<IReadOnlyCollection<string>> GetGuildIdsWithModuleValue(string ruleIdentifier, string key,
        string value)
    {
        using (var session = _provider.OpenSession())
        {
            var query = session.Query<AutoModConfigurationEntity>()
                .Where(entity => entity.RuleKey == ruleIdentifier
                                 && entity.ConfigurationKey == key
                                 && entity.ConfigurationValue == value)
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

    public async Task SetValue(string ruleKey, ulong guildId, string key, string value)
    {
        using (var session = _provider.OpenSession())
        {
            var query = session.Query<AutoModConfigurationEntity>()
                .Where(entity => entity.RuleKey == ruleKey
                                 && entity.GuildId == guildId.ToString()
                                 && entity.ConfigurationKey == key);
            var entity = await query.SingleOrDefaultAsync() ?? new AutoModConfigurationEntity
            {
                ConfigurationKey = key,
                GuildId = guildId.ToString(),
                RuleKey = ruleKey
            };
            entity.ConfigurationValue = value;
            await session.SaveOrUpdateAsync(entity);
            await session.FlushAsync();
        }
    }

    public async Task<string> GetValueAsync(string ruleKey, ulong guildId, string key)
    {
        using (var session = _provider.OpenSession())
        {
            var query = session.Query<AutoModConfigurationEntity>()
                .Where(entity => entity.GuildId == guildId.ToString()
                                 && entity.RuleKey == ruleKey
                                 && entity.ConfigurationKey == key)
                .Select(entity => entity.ConfigurationValue);
            var value = await query.SingleOrDefaultAsync();
            return value;
        }
    }

    public async Task<IReadOnlyCollection<AutoModRuleData>> GetAllConfigsForGuildAsync(string guildId)
    {
        using (var session = _provider.OpenSession())
        {
            var entities = await session.Query<AutoModConfigurationEntity>()
                .Where(entity => entity.GuildId == guildId)
                .ToListAsync();
            return entities.Select(entity =>
                    new AutoModRuleData(entity.RuleKey, entity.ConfigurationKey, entity.ConfigurationValue))
                .ToArray();
        }
    }
}