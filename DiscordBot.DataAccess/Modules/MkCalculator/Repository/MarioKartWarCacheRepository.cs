using System.Linq;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Contract.MkCalculator;
using DiscordBot.DataAccess.Entities;
using DiscordBot.DataAccess.Modules.MkCalculator.Domain;
using DiscordBot.DataAccess.NHibernate;
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
}