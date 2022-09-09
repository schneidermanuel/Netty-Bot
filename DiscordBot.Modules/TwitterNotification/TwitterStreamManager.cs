using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract.TwitterRegistration;
using DiscordBot.DataAccess.Contract.TwitterRegistration.BusinessLogic;
using DiscordBot.Modules.TwitterNotification.TweetDataExtraction;
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
    private readonly IEnumerable<IExtractionStrategy> _extractionStrategies;
    private Task _task;

    public TwitterStreamManager(DiscordSocketClient discordSocketClient, TwitterClient client,
        IList<TwitterRegistrationDto> registrations,
        ITwitterRegistrationBusinessLogic businessLogic,
        TwitterRuleValidator ruleValidator,
        IEnumerable<IExtractionStrategy> extractionStrategies)
    {
        _discordSocketClient = discordSocketClient;
        _client = client;
        _registrations = registrations;
        _businessLogic = businessLogic;
        _ruleValidator = ruleValidator;
        _extractionStrategies = extractionStrategies;
        _listenedUsers = new List<string>();
    }

    public async Task InitializeAsync()
    {
        var regs = (await _businessLogic.RetrieveAllRegistartionsAsync()).ToList();

        _ = Task.Run(async () =>
        {
            while (true)
            {
                try
                {
                    await _client.NextTweetStreamAsync(IncomingTweet, new TweetSearchOptions
                    {
                        UserOptions = Array.Empty<UserOption>()
                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    _client.CancelTweetStream();
                }
            }
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
                var tweetData = ExtractTweetData(tweet);
                builder.WithThumbnailUrl(tweetData.ProfileImage);
                builder.WithDescription(tweetData.Text);
                builder.WithCurrentTimestamp();
                builder.WithTitle(tweetData.Title);
                builder.WithUrl($"https://twitter.com/{tweet.Author.Username}/status/{tweet.Id}");
                builder.WithImageUrl(tweetData.MediaUrl);

                await channel.SendMessageAsync(registration.Message, false, builder.Build());
            }
            catch (Exception)
            {
                Console.WriteLine(
                    $"Registration {registration.GuildId}/{registration.ChannelId}:{registration.Username} could not be processed");
            }
        }
    }

    private TweetData ExtractTweetData(Tweet tweet)
    {
        var strategy = _extractionStrategies.Single(strategy => strategy.IsResponsible(tweet));
        return strategy.Build(tweet);
    }
    
    public async Task RegisterTwitterUserAsync(TwitterRegistrationDto registrationDto)
    {
        var username = registrationDto.Username;
        _registrations.Add(registrationDto);

        if (!_listenedUsers.Contains(username.ToLower()))
        {
            _listenedUsers.Add(username.ToLower());
            try
            {
                await _client.AddTweetStreamAsync(new StreamRequest(Expression.Author(username)));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}