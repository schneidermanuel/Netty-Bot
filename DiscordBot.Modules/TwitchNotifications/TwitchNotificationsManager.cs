﻿using System;
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
    private readonly ITwitchNotificationsBusinessLogic _businessLogic;
    private readonly ITwitchPubsubManager _pubsubManager;
    private readonly DiscordSocketClient _client;
    private List<TwitchNotificationRegistration> _registrations;

    public TwitchNotificationsManager(ITwitchNotificationsBusinessLogic businessLogic,
        ITwitchPubsubManager pubsubManager, DiscordSocketClient client)
    {
        _businessLogic = businessLogic;
        _pubsubManager = pubsubManager;
        _client = client;
        _registrations = new List<TwitchNotificationRegistration>();
    }

    public async Task Initialize()
    {
        var registrations = await _businessLogic.RetrieveAllRegistrationsAsync();
        _registrations.Clear();
        _registrations.AddRange(registrations);
        foreach (var registration in _registrations)
        {
            await _pubsubManager.RegisterStreamerAsync(registration.Streamer);
        }
    }

    public async Task StreamUp(StreamerInformation streamInformation)
    {
        Console.WriteLine("CALLBACL: STREAM UP " + streamInformation.StreamerName);
        var embedBuilder = new EmbedBuilder();
        embedBuilder.WithColor(Color.Purple);
        embedBuilder.WithTitle(streamInformation.StreamTitle);
        embedBuilder.WithDescription(streamInformation.PlayingGame);
        embedBuilder.WithUrl($"https://twitch.tv/{streamInformation.StreamerName}");
        embedBuilder.WithThumbnailUrl(streamInformation.ProfilePictureUrl);
        embedBuilder.WithImageUrl(streamInformation.ThumbnailUrl);
        var embed = embedBuilder.Build();

        foreach (var registration in _registrations.Where(reg => reg.Streamer == streamInformation.StreamerName))
        {
            try
            {
                var channel =
                    (ISocketMessageChannel)_client.GetGuild(registration.GuildId).GetChannel(registration.ChannelId);
                await channel.SendMessageAsync(registration.Message, false, embed);
            }
            catch
            {
                // Ignored
            }
        }
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
    }
}