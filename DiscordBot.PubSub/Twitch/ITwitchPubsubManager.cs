namespace DiscordBot.PubSub.Twitch;

public interface ITwitchPubsubManager
{
    void Initialize(Func<StreamerInformation, Task> callback);
    Task RegisterStreamerAsync(string channelName);
    Task ReconnectAsync();
}
