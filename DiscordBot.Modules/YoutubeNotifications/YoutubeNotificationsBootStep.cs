using System.Threading.Tasks;
using DiscordBot.Framework.Contract.Boot;
using DiscordBot.PubSub.Backend;

namespace DiscordBot.Modules.YoutubeNotifications;

internal class YoutubeNotificationsBootStep : IBootStep
{
    private readonly IDiscordBotPubSubBackendManager _pubSubManager;
    private readonly YoutubeNotificationManager _manager;

    public YoutubeNotificationsBootStep(YoutubeNotificationManager manager,
        IDiscordBotPubSubBackendManager pubSubManager)
    {
        _pubSubManager = pubSubManager;
        _manager = manager;
    }

    public async Task BootAsync()
    {
        _pubSubManager.Run(_manager.Callback);
        await Task.Delay(10000);
        await _manager.InitializeAsync();
    }

    public BootOrder StepPosition => BootOrder.End;
}