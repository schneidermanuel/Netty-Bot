using TwitterSharp.Response.RTweet;

namespace DiscordBot.Modules.TwitterNotification.TweetDataExtraction;

internal interface IExtractionStrategy
{
    bool IsResponsible(Tweet tweet);
    TweetData Build(Tweet tweet);
}