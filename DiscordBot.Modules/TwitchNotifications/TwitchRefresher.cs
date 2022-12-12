using System.Threading.Tasks;
using DiscordBot.Framework.Contract.Modules.TwitchRegistrations;

namespace DiscordBot.Modules.TwitchNotifications;

internal class TwitchRefresher : ITwitchRefresher
{
    private readonly TwitchNotificationsManager _manager;

    public TwitchRefresher(TwitchNotificationsManager manager)
    {
        _manager = manager;
    }
    
    public async Task RefreshAsync(ulong guildId)
    {
        await _manager.RefreshGuildAsync(guildId);
    }
}