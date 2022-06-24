using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract.TwitterRegistration;
using DiscordBot.DataAccess.Contract.TwitterRegistration.BusinessLogic;
using Microsoft.AspNetCore.Components.Web;
using TwitterSharp.Client;
using TwitterSharp.Request;
using TwitterSharp.Request.AdvancedSearch;
using TwitterSharp.Request.Option;
using TwitterSharp.Response.RTweet;
using TwitterSharp.Rule;

// ReSharper disable LocalizableElement

namespace DiscordBot.Modules.TwitterNotification;

internal class TwitterStreamManager
{
    private readonly DiscordSocketClient _discordSocketClient;
    private readonly TwitterClient _client;
    private IList<string> _listenedUsers;
    private IList<TwitterRegistrationDto> _registrations;
    private readonly ITwitterRegistrationBusinessLogic _businessLogic;
    private readonly TwitterRuleValidator _ruleValidator;

    public TwitterStreamManager(DiscordSocketClient discordSocketClient, TwitterClient client,
        IList<TwitterRegistrationDto> registrations,
        ITwitterRegistrationBusinessLogic businessLogic,
        TwitterRuleValidator ruleValidator)
    {
        _discordSocketClient = discordSocketClient;
        _client = client;
        _registrations = registrations;
        _businessLogic = businessLogic;
        _ruleValidator = ruleValidator;
        _listenedUsers = new List<string>();
    }

    public async Task InitializeAsync()
    {
        var regs = (await _businessLogic.RetrieveAllRegistartionsAsync()).ToList();

        Task.Run(async () =>
        {
            await _client.NextTweetStreamAsync(IncomingTweet, new TweetSearchOptions
            {
                UserOptions = Array.Empty<UserOption>()
            });
        });

        foreach (var registration in regs)
        {
            await RegisterTwitterUserAsync(registration);
        }

        Console.WriteLine("Twitter Initialized");
    }

    private async void IncomingTweet(Tweet tweet)
    {
        Console.WriteLine("Tweet here");
        var matchingRegistrations =
            _registrations.Where(reg => reg.Username.ToLower() == tweet.Author.Username.ToLower());
        foreach (var registration in matchingRegistrations)
        {
            try
            {
                if (!_ruleValidator.DoesTweetMatchRule(tweet, registration.RuleFilter))
                {
                    continue;
                }

                var channel = (SocketTextChannel)_discordSocketClient.GetGuild(registration.GuildId)
                    .GetChannel(registration.ChannelId);
                var builder = new EmbedBuilder();
                builder.WithColor(Color.Blue);
                builder.WithThumbnailUrl("https://unavatar.io/twitter/" + tweet.Author.Username);
                var title = BuildTitle(tweet);
                builder.WithDescription(tweet.Text);
                builder.WithCurrentTimestamp();
                builder.WithTitle(title);
                builder.WithUrl($"https://twitter.com/{tweet.Author.Username}/status/{tweet.Id}");
                await channel.SendMessageAsync(registration.Message, false, builder.Build());
            }
            catch (Exception)
            {
                Console.WriteLine(
                    $"Registration {registration.GuildId}/{registration.ChannelId}:{registration.Username} could not be processed");
            }
        }
    }

    private string BuildTitle(Tweet tweet)
    {
        if (Regex.IsMatch(tweet.Text, "^RT @.*:.*"))
        {
            var originalUser = tweet.Text.Split(":")[0].Remove(0, 3);
            return $"{tweet.Author.Name} Retweeted {originalUser}";
        }

        return $"{tweet.Author.Name} Tweeted";
    }

    public async Task RegisterTwitterUserAsync(TwitterRegistrationDto registrationDto)
    {
        var username = registrationDto.Username;
        if (!_listenedUsers.Contains(username.ToLower()))
        {
            var user = await _client.GetUserAsync(username);
            _listenedUsers.Add(username);
            _registrations.Add(registrationDto);
            await _client.AddTweetStreamAsync(new StreamRequest(Expression.Author(user.Username)));
        }
    }
}