using System.Threading.Tasks;
using DiscordBot.Framework.Contract.Modules.YoutubeRegistrations;

namespace DiscordBot.Modules.YoutubeNotifications;

internal class YoutubeRefresher : IYoutubeRefresher
{
    private readonly YoutubeNotificationManager _manager;

    public YoutubeRefresher(YoutubeNotificationManager manager)
    {
        _manager = manager;
    }
    public async Task RefreshGuildAsync(ulong guildId)
    {
        await _manager.RefreshGuildAsync(guildId);
    }
}