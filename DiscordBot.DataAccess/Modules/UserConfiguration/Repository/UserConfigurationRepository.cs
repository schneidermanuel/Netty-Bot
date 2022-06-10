using System.Linq;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Entities;
using DiscordBot.DataAccess.Modules.UserConfiguration.BusinessLogic;
using DiscordBot.DataAccess.NHibernate;
using NHibernate.Linq;

namespace DiscordBot.DataAccess.Modules.UserConfiguration.Repository;

internal class UserConfigurationRepository : IUserConfigurationRepository
{
    private readonly ISessionProvider _sessionProvider;

    public UserConfigurationRepository(ISessionProvider sessionProvider)
    {
        _sessionProvider = sessionProvider;
    }

    public async Task<string> RetrieveConfiguredLanguageAsync(string userId)
    {
        using (var session = _sessionProvider.OpenSession())
        {
            var query = await session.Query<UserConfigurationEntity>()
                .Where(entity => entity.UserId == userId).SingleOrDefaultAsync();
            return query?.LanguageCode;
        }
    }

    public async Task SaveAsync(string userId, string language)
    {
        using (var session = _sessionProvider.OpenSession())
        {
            var config = await session.Query<UserConfigurationEntity>()
                .Where(entity => entity.UserId == userId).SingleOrDefaultAsync() ?? new UserConfigurationEntity();
            config.UserId = userId;
            config.LanguageCode = language;
            await session.SaveOrUpdateAsync(config);
            await session.FlushAsync();
        }
    }
}