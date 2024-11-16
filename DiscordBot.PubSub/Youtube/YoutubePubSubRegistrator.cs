using DiscordBot.Framework.Contract;
using DiscordBot.PubSub.Backend;

namespace DiscordBot.PubSub.Youtube;

internal class YoutubePubSubRegistrator : IYoutubePubSubRegistrator
{
    private const string SubscribeUrl = "https://pubsubhubbub.appspot.com/subscribe";

    public async Task SubscribeAsync(string channelId)
    {
        var topic = $"https://www.youtube.com/feeds/videos.xml?channel_id={channelId}";

        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("ContentType", "application/x-www-form-urlencoded");
        var response = await client.PostAsync(SubscribeUrl, new FormUrlEncodedContent(
            new List<KeyValuePair<string, string>>
            {
                new("hub.mode", "subscribe"),
                new("hub.callback", $"https://{BotClientConstants.Hostname}:{BotClientConstants.Port}"),
                new("hub.topic", topic),
                new("hub.verify", "async"),
                new("hub.secret", "hello")
            }));


        var responseData = await response.Content.ReadAsStringAsync();
        Console.WriteLine(responseData);
    }
}