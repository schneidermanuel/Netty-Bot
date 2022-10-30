using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract.YoutubeNotification;
using DiscordBot.Framework.Contract;
using DiscordBot.PubSub.Backend.Data;
using DiscordBot.PubSub.Youtube;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;

namespace DiscordBot.Modules.YoutubeNotifications;

internal class YoutubeNotificationManager
{
    private readonly IYoutubeNotificationDomain _domain;
    private readonly IYoutubePubSubRegistrator _registrator;
    private readonly DiscordSocketClient _client;
    private YouTubeService _api;
    private List<YoutubeNotificationRegistration> _registrations;
    private List<string> _cache;

    public YoutubeNotificationManager(IYoutubeNotificationDomain domain,
        IYoutubePubSubRegistrator registrator,
        DiscordSocketClient client)
    {
        _domain = domain;
        _registrator = registrator;
        _client = client;
    }

    public async Task InitializeAsync()
    {
        _api = new YouTubeService(new BaseClientService.Initializer()
        {
            ApiKey = BotClientConstants.YoutubeApiKey,
            ApplicationName = GetType().ToString()
        });
        _registrations = new List<YoutubeNotificationRegistration>();
        _cache = new List<string>();
        await RefreshAllRegistrations();
    }

    public string GetUsernameById(string id)
    {
        var request = _api.Channels.List("snippet");
        request.Id = id;
        var result = request.Execute();
        return result.Items?.First().Snippet.Title;
    }

    public async Task RegisterChannelAsync(YoutubeNotificationRegistration registration)
    {
        _registrations.Add(registration);
        await _registrator.SubscribeAsync(registration.YoutubeChannelId);
    }

    public async Task RefreshAllRegistrations()
    {
        _registrations.Clear();
        var registrations = await _domain.RetrieveAllRegistrationsAsync();
        _registrations.AddRange(registrations);
        foreach (var registration in _registrations)
        {
            await _registrator.SubscribeAsync(registration.YoutubeChannelId);
        }

        _cache.Clear();
    }

    public void RemoveRegistration(string youtubeChannelId, ulong guildId)
    {
        _registrations.RemoveAll(reg => reg.YoutubeChannelId == youtubeChannelId && reg.GuildId == guildId);
    }

    public async Task Callback(YoutubeNotification notification)
    {
        Console.WriteLine(notification.IsNewVideo);
        if (_cache.Contains(notification.VideoId))
        {
            return;
        }

        _cache.Add(notification.VideoId);
        var responsibleRegistrations =
            _registrations.Where(reg => reg.YoutubeChannelId == notification.ChannelId).ToArray();

        var userRequest = _api.Channels.List("snippet");
        userRequest.Id = notification.ChannelId;
        var userResultTask = userRequest.ExecuteAsync();

        var videoRequest = _api.Videos.List("snippet");
        Console.WriteLine("CALLBACK");
        Console.WriteLine(notification.VideoId);
        videoRequest.Id = notification.VideoId;
        var videoRequestTask = videoRequest.ExecuteAsync();

        var userResult = await userResultTask;
        var videoResult = await videoRequestTask;
        var video = videoResult.Items?.First();
        if (video == null)
        {
            return;
        }

        var user = userResult.Items?.First();
        if (user == null)
        {
            return;
        }

        var embedBuilder = new EmbedBuilder();
        embedBuilder.WithCurrentTimestamp();
        embedBuilder.WithColor(Color.Red);
        embedBuilder.WithTitle(notification.Title);
        embedBuilder.WithUrl($"http://www.youtube.com/watch?v={notification.VideoId}");
        embedBuilder.WithThumbnailUrl(user.Snippet.Thumbnails.High.Url);
        embedBuilder.WithImageUrl(video.Snippet.Thumbnails.Medium.Url);
        var description = video.Snippet.Description;
        if (description.Length > 30)
        {
            description = description.Substring(0, 28) + "...";
        }

        embedBuilder.WithDescription(description);
        var embed = embedBuilder.Build();
        foreach (var registration in responsibleRegistrations)
        {
            try
            {
                var channel =
                    (ISocketMessageChannel)_client.GetGuild(registration.GuildId).GetChannel(registration.ChannelId);
                await channel.SendMessageAsync(registration.Message, false, embed);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}