namespace DiscordBot.PubSub.Youtube;

public interface IYoutubePubSubRegistrator
{
    Task SubscribeAsync(string channelId);
}