using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract.TwitchNotifications;
using DiscordBot.PubSub.Twitch;

namespace DiscordBot.Modules.TwitchNotifications;

internal class TwitchNotificationsManager
{
    private readonly ITwitchNotificationsDomain _domain;
    private readonly ITwitchPubsubManager _pubsubManager;
    private readonly DiscordSocketClient _client;
    private List<TwitchNotificationRegistration> _registrations;

    public TwitchNotificationsManager(ITwitchNotificationsDomain domain,
        ITwitchPubsubManager pubsubManager, DiscordSocketClient client)
    {
        _domain = domain;
        _pubsubManager = pubsubManager;
        _client = client;
        _registrations = new List<TwitchNotificationRegistration>();
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

        await _pubsubManager.ReconnectAsync();
    }

    public async Task StreamUp(StreamerInformation streamInformation)
    {
        foreach (var registration in _registrations.Where(reg =>
                     reg.Streamer.ToLower() == streamInformation.StreamerName.ToLower()))
        {
            try
            {
                Console.WriteLine("CALLBACK: STREAM UP " + streamInformation.StreamerName);
                var channel =
                    (ISocketMessageChannel)_client.GetGuild(registration.GuildId).GetChannel(registration.ChannelId);
                var embed = BuildEmbed(streamInformation);

                await channel.SendMessageAsync(registration.Message, false, embed);
            }
            catch
            {
                // Ignored
            }
        }
    }

    private static Embed BuildEmbed(StreamerInformation streamInformation)
    {
        var embedBuilder = new EmbedBuilder();
        embedBuilder.WithColor(Color.Purple);
        embedBuilder.WithTitle(streamInformation.StreamTitle);
        embedBuilder.WithDescription(streamInformation.PlayingGame);
        embedBuilder.WithUrl($"https://twitch.tv/{streamInformation.StreamerName}");
        embedBuilder.WithThumbnailUrl(streamInformation.ProfilePictureUrl);
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

    public async Task AddUserAsync(TwitchNotificationRegistration registration)
    {
        _registrations.Add(registration);
        await _pubsubManager.RegisterStreamerAsync(registration.Streamer);
        await _pubsubManager.ReconnectAsync();
    }
}