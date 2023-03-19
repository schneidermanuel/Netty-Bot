using System.Text.RegularExpressions;
using TwitterSharp.Response.RTweet;

namespace DiscordBot.Modules.TwitterNotification;

internal static class TwitterRegexHelper
{
    public static bool IsAnswer(this Tweet tweet)
    {
        return Regex.Match(tweet.Text, @"^@.*").Success;
    }

    public static bool IsRetweet(this Tweet tweet)
    {
        return Regex.Match(tweet.Text, @"^RT @.*\:.*").Success;
    }

}