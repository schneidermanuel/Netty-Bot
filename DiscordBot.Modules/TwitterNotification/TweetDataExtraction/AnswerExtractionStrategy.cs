using System;
using System.Linq;
using TwitterSharp.Response.RTweet;

namespace DiscordBot.Modules.TwitterNotification.TweetDataExtraction;

internal class AnswerExtractionStrategy : IExtractionStrategy
{
    public bool IsResponsible(Tweet tweet)
    {
        return tweet.IsAnswer() && !tweet.IsRetweet();
    }

    public TweetData Build(Tweet tweet)
    {
        var data = new TweetData();

        var originalUser = tweet.Text.Split(" ")[0].Remove(0, 1);
        data.Title = $"{tweet.Author.Name} replied to {originalUser}";
        data.ProfileImage =
            $"https://unavatar.io/twitter/{tweet.Author.Name}?fallback={DateTimeOffset.Now.ToUnixTimeSeconds()}";
        data.Text = tweet.Text.Substring(tweet.Text.IndexOf(' ') + 1);
        data.MediaUrl = tweet.Attachments.Media.Any() ? tweet.Attachments.Media.First().Url : string.Empty;
        return data;
    }
}