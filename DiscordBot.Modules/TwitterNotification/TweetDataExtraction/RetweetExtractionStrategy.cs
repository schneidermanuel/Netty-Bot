using System;
using System.Linq;
using TwitterSharp.Response.RTweet;

namespace DiscordBot.Modules.TwitterNotification.TweetDataExtraction;

internal class RetweetExtractionStrategy : IExtractionStrategy
{
    public bool IsResponsible(Tweet tweet)
    {
        return tweet.IsRetweet() && !tweet.IsAnswer();
    }

    public TweetData Build(Tweet tweet)
    {
        var data = new TweetData();

        var originalUser = tweet.Text.Split(":")[0].Remove(0, 3);
        data.Title = $"{tweet.Author.Name} Retweeted {originalUser}";
        data.ProfileImage =
            $"https://unavatar.io/twitter/{originalUser}?fallback={DateTimeOffset.Now.ToUnixTimeSeconds()}";
        data.Text = tweet.Text.Substring(tweet.Text.IndexOf(':') + 1);
        data.MediaUrl = tweet.Attachments.Media.Any() ? tweet.Attachments.Media.First().Url : string.Empty;
        return data;
    }
}