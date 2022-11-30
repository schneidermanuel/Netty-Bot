using DiscordBot.Framework.Contract;
using TwitchLib.Api;
using TwitchLib.PubSub;
using TwitchLib.PubSub.Events;

namespace DiscordBot.PubSub.Twitch;

internal class TwitchPubsubManager : ITwitchPubsubManager
{
    private TwitchPubSub _client;
    private TwitchAPI _api;
    private Func<StreamerInformation, Task> _callback;
    private List<string> _listening;
    private Dictionary<string, DateTime> _timeout;

    public void Initialize(Func<StreamerInformation, Task> callback)
    {
        _timeout = new Dictionary<string, DateTime>();
        _listening = new List<string>();
        _client = new TwitchPubSub();
        _client.OnPubSubServiceConnected += ServerConnected;
        _client.OnStreamUp += StreamUp;

        _client.Connect();

        _api = new TwitchAPI();
        _api.Settings.ClientId = BotClientConstants.TwitchClientId;
        _api.Settings.Secret = BotClientConstants.TwitchClientSecret;
        var token = _api.Auth.GetAccessToken();
        _api.Settings.AccessToken = token;
        _callback = callback;
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
                _client.ListenToVideoPlayback(id);
                _listening.Add(id);
                Console.WriteLine("[Twitch] Listening to " + channelName);
                _client.Disconnect();
                _client.Connect();
            }
        }
        catch (Exception)
        {
            Console.WriteLine("[TWITCH] Error Listening to " + channelName);
        }
    }

    public async Task ReconnectAsync()
    {
        _client.Disconnect();
        Console.WriteLine("[Twitch] Reconnecting...");
        _client = new TwitchPubSub();
        _client.OnPubSubServiceConnected += ServerConnected;
        _client.OnStreamUp += StreamUp;

        _client.Connect();
        foreach (var listening in _listening)
        {
            _client.ListenToVideoPlayback(listening);
        }

        await Task.CompletedTask;
    }

    private async void StreamUp(object sender, OnStreamUpArgs e)
    {
        var data = await _api.Helix.Channels.GetChannelInformationAsync(e.ChannelId);
        if (!data.Data.Any())
        {
            throw new InvalidOperationException();
        }

        var channel = data.Data.First();

        if (_timeout.ContainsKey(channel.BroadcasterName) &&
            _timeout[channel.BroadcasterName] > DateTime.Now.AddMinutes(-30))
        {
            Console.WriteLine("In Timeout - not sendung");
            return;
        }

        if (!_timeout.ContainsKey(channel.BroadcasterName))
        {
            _timeout.Add(channel.BroadcasterName, DateTime.Now);
        }

        _timeout[channel.BroadcasterName] = DateTime.Now;

        var userResponse = await _api.Helix.Users.GetUsersAsync(new List<string> { channel.BroadcasterId });
        Console.WriteLine("STREAM UP: " + channel.BroadcasterName);
        if (!userResponse.Users.Any())
        {
            throw new InvalidOperationException();
        }

        var user = userResponse.Users.First();

        var timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();
        var thumbnail =
            $"https://static-cdn.jtvnw.net/previews-ttv/live_user_{channel.BroadcasterName.ToLower()}-320x180.jpg?r={timestamp}";
        var information = new StreamerInformation
        {
            PlayingGame = channel.GameName,
            StreamerName = channel.BroadcasterName,
            StreamTitle = channel.Title,
            ThumbnailUrl = thumbnail,
            ProfilePictureUrl = user.ProfileImageUrl
        };
        await _callback.Invoke(information);
    }

    private void ServerConnected(object sender, EventArgs e)
    {
        _client.SendTopics();
    }
}