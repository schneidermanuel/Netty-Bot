using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Entities;
using DiscordBot.DataAccess.Modules.TwitterRegistration.BusinessLogic;
using DiscordBot.DataAccess.NHibernate;
using NHibernate.Linq;

namespace DiscordBot.DataAccess.Modules.TwitterRegistration.Repository;

internal class TwitterRegistrationRepository : ITwitterRegistrationRepository
{
    private readonly ISessionProvider _sessionProvider;

    public TwitterRegistrationRepository(ISessionProvider sessionProvider)
    {
        _sessionProvider = sessionProvider;
    }

    public async Task<bool> IsAccountRegisteredOnChannelAsync(string guildId, string channelId, string username)
    {
        using (var session = _sessionProvider.OpenSession())
        {
            var query = session.Query<TwitterRegistrationEntity>()
                .Where(entity => entity.GuildId == guildId && entity.ChannelId == channelId &&
                                 username == entity.TwitterUsername);
            return await query.AnyAsync();
        }
    }

    public async Task RegisterTwitterAsync(TwitterRegistrationData data)
    {
        using (var session = _sessionProvider.OpenSession())
        {
            var entity = new TwitterRegistrationEntity
            {
                ChannelId = data.ChannelId,
                GuildId = data.GuildId,
                RegistrationId = 0,
                TwitterUsername = data.TwitterUsername,
                Message = data.Message,
                RuleFilter = data.RuleFilter
            };
            await session.SaveOrUpdateAsync(entity);
            await session.FlushAsync();
        }
    }

    public async Task<IReadOnlyCollection<TwitterRegistrationData>> RetrieveAllTwitterRegistrationsAsync()
    {
        using (var session = _sessionProvider.OpenSession())
        {
            var query = await session.Query<TwitterRegistrationEntity>().ToListAsync();
            return query.Select(reg =>
                    new TwitterRegistrationData(reg.RegistrationId, reg.GuildId, reg.ChannelId, reg.TwitterUsername, reg.Message, reg.RuleFilter))
                .ToArray();
        }
    }
}