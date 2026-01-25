using DiscordBot.Framework.Contract;
using DiscordBot.PubSub.Backend;

namespace DiscordBot.PubSub.Youtube;

internal class YoutubePubSubRegistrator : IYoutubePubSubRegistrator
{
    private const string SubscribeUrl = "https://pubsubhubbub.appspot.com/subscribe";

    public async Task SubscribeAsync(string channelId)
    {
        var topic = $"https://www.youtube.com/xml/feeds/videos.xml?channel_id={channelId}";

        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("ContentType", "application/x-www-form-urlencoded");
        var response = await client.PostAsync(SubscribeUrl, new FormUrlEncodedContent(
            new List<KeyValuePair<string, string>>
            {
                new("hub.mode", "subscribe"),
                new("hub.callback", $"https://{BotClientConstants.Hostname}"),
                new("hub.topic", topic),
                new("hub.verify", "async"),
                new("hub.verify_token", Guid.NewGuid().ToString()),
                new("hub.secret", PubSubSecret.Secret)
            }));


        var responseData = await response.Content.ReadAsStringAsync();
        Console.WriteLine("Subscribing to Youtube hub: ");
        Console.WriteLine("token: " + PubSubSecret.Secret);
        Console.WriteLine("topic: " + topic);
        Console.WriteLine(responseData);
    }
}