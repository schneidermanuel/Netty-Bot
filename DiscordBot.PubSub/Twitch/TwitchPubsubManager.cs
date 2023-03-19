using DiscordBot.Framework.Contract;
using DiscordBot.PubSub.Backend;
using TwitchLib.Api;

namespace DiscordBot.PubSub.Twitch;

internal class TwitchPubsubManager : ITwitchPubsubManager
{
    private TwitchAPI _api;
    private List<string> _listening;

    public async Task Initialize()
    {
        _listening = new List<string>();

        _api = new TwitchAPI();
        _api.Settings.ClientId = BotClientConstants.TwitchClientId;
        _api.Settings.Secret = BotClientConstants.TwitchClientSecret;
        var token = _api.Auth.GetAccessToken();
        _api.Settings.AccessToken = token;

        await Cleanup();
    }

    private async Task Cleanup()
    {
        var currentRegistrations = await _api.Helix.EventSub.GetEventSubSubscriptionsAsync(clientId: BotClientConstants.TwitchClientId, accessToken:_api.Settings.AccessToken);
        foreach (var registration in currentRegistrations.Subscriptions)
        {
            using (var http = new HttpClient())
            {
                http.DefaultRequestHeaders.Add("Authorization", $"Bearer {_api.Settings.AccessToken}");
                http.DefaultRequestHeaders.Add("Client-Id", BotClientConstants.TwitchClientId);
                await http.DeleteAsync("https://api.twitch.tv/helix/eventsub/subscriptions?id=" + registration.Id);
            }
        }
    }

    public async Task RegisterStreamerAsync(string channelName)
    {
        try
        {
            var channelInfo = await _api.Helix.Users.GetUsersAsync(null, new List<string> { channelName });
            if (!channelInfo.Users.Any())
            {
                return;
            }

            var id = channelInfo.Users.First().Id;
            if (!_listening.Contains(id))
            {
                var conditions = new Dictionary<string, string>();
                conditions.Add("broadcaster_user_id", id);
                await _api.Helix.EventSub.CreateEventSubSubscriptionAsync("stream.online", "1", conditions, "webhook",
                    "https://callback.netty-bot.com/", PubSubSecret.Secret, BotClientConstants.TwitchClientId);
                _listening.Add(id);
                Console.WriteLine("[Twitch] Listening to " + channelName);
            }
        }
        catch (Exception)
        {
            Console.WriteLine("[TWITCH] Error Listening to " + channelName);
        }
    }
}