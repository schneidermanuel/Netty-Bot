using DiscordBot.PubSub.Backend.Data;

namespace DiscordBot.PubSub.Backend;

public interface IDiscordBotPubSubBackendManager
{
    void Run(Func<YoutubeNotification, Task> youtubeCallback, Func<string, Task> callback);
}