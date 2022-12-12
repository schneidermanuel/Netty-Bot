using System.Threading.Tasks;
using DiscordBot.Framework.Contract.Boot;
using DiscordBot.PubSub.Backend;

namespace DiscordBot.Modules.YoutubeNotifications;

internal class YoutubeNotificationsBootStep : IBootStep
{
    private readonly YoutubeNotificationManager _manager;

    public YoutubeNotificationsBootStep(YoutubeNotificationManager manager)
    {
        _manager = manager;
    }

    public async Task BootAsync()
    {
        await _manager.InitializeAsync();
    }

    public BootOrder StepPosition => BootOrder.End;
}