using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TwitterSharp.Response.RTweet;

namespace DiscordBot.Modules.TwitterNotification;

internal class TwitterRuleValidator
{
    private static readonly IEnumerable<string> _keywords = new[]
        { IsRetweetKey, IsReply, IsNormalKey, NotKey };

    private static readonly IEnumerable<string> _parameterKeywords = new[] { ContainsKey };

    private const string NotKey = "NOT";
    private const string IsNormalKey = "IS_NORMAL";
    private const string IsReply = "IS_REPLY";
    private const string IsRetweetKey = "IS_RETWEET";
    private const string ContainsKey = "CONTAINS";


    public bool IsRuleValid(string rule)
    {
        var ruleParts = rule.Split(' ');

        for (var i = 0; i < ruleParts.Length; i++)
        {
            if (ruleParts[i].Trim() == string.Empty)
            {
                continue;
            }

            if (_parameterKeywords.Contains(ruleParts[i]))
            {
                i++;
                continue;
            }

            if (!_keywords.Contains(ruleParts[i]))
            {
                return false;
            }
        }

        return true;
    }

    public bool DoesTweetMatchRule(Tweet tweet, string rule)
    {
        if (string.IsNullOrEmpty(rule))
        {
            return true;
        }

        var ruleParts = rule.Split(' ');
        var isInverted = false;
        for (var i = 0; i < ruleParts.Length; i++)
        {
            if (ruleParts[i].Trim() == string.Empty)
            {
                continue;
            }

            switch (ruleParts[i])
            {
                case IsNormalKey:
                    if ((tweet.IsRetweet() || tweet.IsAnswer()) ^ isInverted)
                    {
                        return false;
                    }

                    break;
                case IsReply:
                    if (!tweet.IsAnswer() ^ isInverted)
                    {
                        return false;
                    }

                    break;
                case IsRetweetKey:
                    if (!tweet.IsRetweet() ^ isInverted)
                    {
                        return false;
                    }

                    break;
                case NotKey:
                    isInverted = true;
                    continue;

                case ContainsKey:
                    i++;
                    var word = ruleParts[i];
                    if (!tweet.Text.ToLower().Contains(word.ToLower()) ^ isInverted)
                    {
                        return false;
                    }

                    break;
            }

            isInverted = false;
        }

        return true;
    }
}