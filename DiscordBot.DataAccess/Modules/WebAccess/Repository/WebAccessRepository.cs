using System.Linq;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Entities;
using DiscordBot.DataAccess.Hibernate;
using DiscordBot.DataAccess.Modules.WebAccess.Domain;
using NHibernate.Linq;

namespace DiscordBot.DataAccess.Modules.WebAccess.Repository;

internal class WebAccessRepository : IWebAccessRepository
{
    private readonly ISessionProvider _sessionProvider;

    public WebAccessRepository(ISessionProvider sessionProvider)
    {
        _sessionProvider = sessionProvider;
    }

    public async Task<string> GetValue(string key)
    {
        using (var session = _sessionProvider.OpenSession())
        {
            var entity = await session.LoadAsync<WebConfigEntity>(key);
            return entity.Value;
        }
    }

    public async Task UpdateValue(string key, string value)
    {
        using (var session = _sessionProvider.OpenSession())
        {
            var webConfig = await session.Query<WebConfigEntity>()
                                .Where(entity => entity.Identifier == key)
                                .SingleOrDefaultAsync()
                            ?? new WebConfigEntity
                            {
                                Identifier = key,
                            };
            webConfig.Value = value;
            await session.SaveOrUpdateAsync(webConfig);
            await session.FlushAsync();
        }
    }
}