using System.Threading.Tasks;
using DiscordBot.Framework.Contract.Boot;
using DiscordBot.Modules.TwitchNotifications;
using DiscordBot.Modules.YoutubeNotifications;
using DiscordBot.PubSub.Backend;

namespace DiscordBot.Modules.WebBackend;

internal class BackendStarter : IBootStep
{
    private readonly YoutubeNotificationManager _youtubeNotificationManager;
    private readonly TwitchNotificationsManager _twitchNotificationsManager;
    private readonly IDiscordBotPubSubBackendManager _pubSubBackendManager;

    public BackendStarter(YoutubeNotificationManager youtubeNotificationManager,
        TwitchNotificationsManager twitchNotificationsManager, IDiscordBotPubSubBackendManager pubSubBackendManager)
    {
        _youtubeNotificationManager = youtubeNotificationManager;
        _twitchNotificationsManager = twitchNotificationsManager;
        _pubSubBackendManager = pubSubBackendManager;
    }

    public async Task BootAsync()
    {
        _pubSubBackendManager.Run(_youtubeNotificationManager.Callback, _twitchNotificationsManager.Callback);
        await Task.CompletedTask;
    }

    public BootOrder StepPosition => BootOrder.First;
}