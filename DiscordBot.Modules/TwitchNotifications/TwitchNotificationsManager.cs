using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract.TwitchNotifications;
using DiscordBot.Framework.Contract;
using DiscordBot.PubSub.Twitch;
using TwitchLib.Api;
using TwitchLib.Api.Helix.Models.Streams.GetStreams;
using TwitchLib.Api.Helix.Models.Users.GetUsers;

namespace DiscordBot.Modules.TwitchNotifications;

internal class TwitchNotificationsManager
{
    private readonly ITwitchNotificationsDomain _domain;
    private readonly ITwitchPubsubManager _pubsubManager;
    private readonly DiscordSocketClient _client;
    private List<TwitchNotificationRegistration> _registrations;
    private readonly TwitchAPI _api;

    public TwitchNotificationsManager(ITwitchNotificationsDomain domain,
        ITwitchPubsubManager pubsubManager, DiscordSocketClient client)
    {
        _domain = domain;
        _pubsubManager = pubsubManager;
        _client = client;
        _registrations = new List<TwitchNotificationRegistration>();
        _api = new TwitchAPI();
        _api.Settings.ClientId = BotClientConstants.TwitchClientId;
        _api.Settings.Secret = BotClientConstants.TwitchClientSecret;
        var token = _api.Auth.GetAccessToken();
        _api.Settings.AccessToken = token;
    }

    public Func<string, Task> Callback => CallbackTwitch;

    private async Task CallbackTwitch(string username)
    {
        Console.WriteLine("CALLBACK: STREAM UP " + username);

        var infos = await _api.Helix.Streams.GetStreamsAsync(userLogins: new List<string> { username });
        var users = await _api.Helix.Users.GetUsersAsync(logins: new List<string> { username });
        var stream = infos.Streams.Single();
        var user = users.Users.Single();

        foreach (var registration in _registrations.Where(reg =>
                     string.Equals(reg.Streamer, username, StringComparison.CurrentCultureIgnoreCase)))
        {
            try
            {
                var channel =
                    (ISocketMessageChannel)_client.GetGuild(registration.GuildId).GetChannel(registration.ChannelId);
                var embed = BuildEmbed(stream, user);

                await channel.SendMessageAsync(registration.Message, false, embed);
            }
            catch  (Exception e)
            {
                Console.WriteLine("Twitch exception");
                Console.WriteLine(e);
                Console.WriteLine(e.StackTrace);
            }
        }
    }

    public async Task Initialize()
    {
        var registrations = await _domain.RetrieveAllRegistrationsAsync();
        _registrations.Clear();
        _registrations.AddRange(registrations);
        foreach (var registration in _registrations)
        {
            await _pubsubManager.RegisterStreamerAsync(registration.Streamer);
        }
    }

    private static Embed BuildEmbed(Stream streamInformation, User user)
    {
        var embedBuilder = new EmbedBuilder();
        embedBuilder.WithColor(Color.Purple);
        embedBuilder.WithTitle(streamInformation.Title);
        embedBuilder.WithDescription(streamInformation.GameName);
        embedBuilder.WithUrl($"https://twitch.tv/{streamInformation.UserName}");
        embedBuilder.WithThumbnailUrl(user.ProfileImageUrl);
        embedBuilder.WithImageUrl(streamInformation.ThumbnailUrl);
        var embed = embedBuilder.Build();
        return embed;
    }

    public void RemoveUser(string username, ulong guildId)
    {
        var registrationsToDelete = _registrations
            .Where(reg => reg.Streamer == username
                          && reg.GuildId == guildId);
        foreach (var registration in registrationsToDelete)
        {
            _registrations.Remove(registration);
        }
    }

    public async Task RefreshGuildAsync(ulong guildId)
    {
        var registrations = _registrations.Where(reg => reg.GuildId == guildId).ToArray();
        foreach (var registration in registrations)
        {
            _registrations.Remove(registration);
        }

        var guildRegistrations = await _domain.RetrieveAllRegistrationsForGuildAsync(guildId);
        foreach (var guildRegistration in guildRegistrations)
        {
            await AddUserAsync(guildRegistration);
        }
    }

    public async Task AddUserAsync(TwitchNotificationRegistration registration)
    {
        _registrations.Add(registration);
        await _pubsubManager.RegisterStreamerAsync(registration.Streamer);
    }
}