using System.Linq;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Entities;
using DiscordBot.DataAccess.Modules.GuildConfig.Domain;
using DiscordBot.DataAccess.NHibernate;
using NHibernate.Linq;

namespace DiscordBot.DataAccess.Modules.GuildConfig.Repository;

internal class GuildConfigRepository : IGuildConfigRepository
{
    private readonly ISessionProvider _sessionProvider;

    public GuildConfigRepository(ISessionProvider sessionProvider)
    {
        _sessionProvider = sessionProvider;
    }

    public async Task<char> GetPrefixAsync(string guidlId)
    {
        using (var session = _sessionProvider.OpenSession())
        {
            var config = await session.Query<GuildConfigEntity>()
                .Where(entity => entity.GuildId == guidlId).SingleOrDefaultAsync();
            return config == null ? '!' : char.Parse(config.Prefix);
        }
    }

    public async Task SavePrefixAsync(string guildId, string prefix)
    {
        using (var session = _sessionProvider.OpenSession())
        {
            var config = (await session.Query<GuildConfigEntity>()
                .Where(entity => entity.GuildId == guildId).SingleOrDefaultAsync()) ?? new GuildConfigEntity();
            config.GuildId = guildId;
            config.Prefix = prefix;
            await session.SaveOrUpdateAsync(config);
            await session.FlushAsync();
        }
    }
}