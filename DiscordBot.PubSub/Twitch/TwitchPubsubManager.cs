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

    public void Initialize(Func<StreamerInformation, Task> callback)
    {
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

    public async Task<bool> RegisterStreamerAsync(string channelName)
    {
        var channelInfo = await _api.Helix.Users.GetUsersAsync(null, new List<string> { channelName });
        if (!channelInfo.Users.Any())
        {
            return false;
        }

        var id = channelInfo.Users.First().Id;
        if (!_listening.Contains(id))
        {
            _client.ListenToVideoPlayback(id);
            _client.Disconnect();
            _client.Connect();
            _listening.Add(id);
        }

        return true;
    }

    private async void StreamUp(object? sender, OnStreamUpArgs e)
    {
        var data = await _api.Helix.Channels.GetChannelInformationAsync(e.ChannelId);
        if (!data.Data.Any())
        {
            throw new InvalidOperationException();
        }

        var channel = data.Data.First();
        var userResponse = await _api.Helix.Users.GetUsersAsync(new List<string> { channel.BroadcasterId });
        if (!userResponse.Users.Any())
        {
            throw new InvalidOperationException();
        }

        var user = userResponse.Users.First();

        var thumbnail = $"https://static-cdn.jtvnw.net/previews-ttv/live_user_{channel.BroadcasterName}-1920x1080.jpg";
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

    private void ServerConnected(object? sender, EventArgs e)
    {
        _client.SendTopics();
    }
}