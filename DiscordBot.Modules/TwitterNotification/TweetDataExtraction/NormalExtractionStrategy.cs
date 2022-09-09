using System;
using System.Linq;
using TwitterSharp.Response.RTweet;

namespace DiscordBot.Modules.TwitterNotification.TweetDataExtraction;

internal class NormalExtractionStrategy : IExtractionStrategy
{
    public bool IsResponsible(Tweet tweet)
    {
        return !tweet.IsAnswer() && !tweet.IsRetweet();
    }

    public TweetData Build(Tweet tweet)
    {
        var data = new TweetData();
        data.Title = $"{tweet.Author.Name} Tweeted";
        data.ProfileImage =
            $"https://unavatar.io/twitter/{tweet.Author.Name}?fallback={DateTimeOffset.Now.ToUnixTimeSeconds()}";
        data.Text = tweet.Text;
        data.MediaUrl = tweet.Attachments.Media.Any() ? tweet.Attachments.Media.First().Url : string.Empty;
        return data;

    }
}