namespace DiscordBot.PubSub.Twitch;

public interface ITwitchPubsubManager
{
    void Initialize(Func<StreamerInformation, Task> callback);
    Task<bool> RegisterStreamerAsync(string channelName);
    Task ReconnectAsync();
}
