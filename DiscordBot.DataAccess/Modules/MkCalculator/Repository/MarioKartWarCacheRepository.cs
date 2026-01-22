using System.Linq;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Contract.MkCalculator;
using DiscordBot.DataAccess.Entities;
using DiscordBot.DataAccess.Hibernate;
using DiscordBot.DataAccess.Modules.MkCalculator.Domain;
using NHibernate.Linq;

namespace DiscordBot.DataAccess.Modules.MkCalculator.Repository;

internal class MarioKartWarCacheRepository : IMarioKartWarCacheRepository
{
    private readonly ISessionProvider _provider;

    public MarioKartWarCacheRepository(ISessionProvider provider)
    {
        _provider = provider;
    }

    public async Task<MarioKartWarRegistry> RetrieveCachedRegistryAsync(string guildId)
    {
        using (var session = _provider.OpenSession())
        {
            var query = await session.Query<MarioKartGuildCacheEntity>()
                .Where(entity => entity.GuildId == guildId)
                .SingleOrDefaultAsync();
            if (query == null)
            {
                return new MarioKartWarRegistry(string.Empty, string.Empty, string.Empty, string.Empty);
            }

            return new MarioKartWarRegistry(query.TeamName, query.TeamImage, query.EnemyName, query.EnemyImage);
        }
    }

    public async Task SaveTeamsAsync(MarioKartWarRegistry marioKartWarRegistry, string guildId)
    {
        using (var session = _provider.OpenSession())
        {
            var query = await session.Query<MarioKartGuildCacheEntity>()
                .Where(entity => entity.GuildId == guildId)
                .SingleOrDefaultAsync() ?? new MarioKartGuildCacheEntity
            {
                GuildId = guildId
            };
            query.TeamName = marioKartWarRegistry.TeamName;
            query.TeamImage = marioKartWarRegistry.TeamImage;
            query.EnemyName = marioKartWarRegistry.EnemyName;
            query.EnemyImage = marioKartWarRegistry.EnemyImage;
            await session.SaveOrUpdateAsync(query);
            await session.FlushAsync();
        }
    }
}