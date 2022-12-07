using System.Threading.Tasks;
using DiscordBot.Framework.Contract.Modules.Twitter;

namespace DiscordBot.Modules.TwitterNotification;

internal class TwitterRefresher : ITwitterRefresher
{
    private readonly TwitterStreamManager _twitterStreamManager;

    public TwitterRefresher(TwitterStreamManager twitterStreamManager)
    {
        _twitterStreamManager = twitterStreamManager;
    }

    public async Task RefreshTwitterAsync(ulong guildId)
    {
        
    }
}