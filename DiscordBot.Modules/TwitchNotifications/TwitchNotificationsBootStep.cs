using System.Threading.Tasks;
using DiscordBot.Framework.Contract.Boot;
using DiscordBot.PubSub.Twitch;

namespace DiscordBot.Modules.TwitchNotifications;

internal class TwitchNotificationsBootStep : IBootStep
{
    private readonly ITwitchPubsubManager _pubsub;
    private readonly TwitchNotificationsManager _manager;

    public TwitchNotificationsBootStep(ITwitchPubsubManager pubsub,
        TwitchNotificationsManager manager)
    {
        _pubsub = pubsub;
        _manager = manager;
    }

    public async Task BootAsync()
    {
        await _pubsub.Initialize();
        await _manager.Initialize();
    }

    public BootOrder StepPosition => BootOrder.End;
}