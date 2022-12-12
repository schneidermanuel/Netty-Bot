namespace DiscordBot.PubSub.Twitch;

public interface ITwitchPubsubManager
{
    Task Initialize();
    Task RegisterStreamerAsync(string channelName);
}
