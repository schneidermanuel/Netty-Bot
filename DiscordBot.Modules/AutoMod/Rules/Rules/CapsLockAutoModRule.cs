using System.Collections.Generic;
using System.Linq;
using Discord.Commands;
using DiscordBot.DataAccess.Contract.AutoMod;
using DiscordBot.DataAccess.Contract.AutoMod.Violation;
using DiscordBot.Framework.Extentions;

namespace DiscordBot.Modules.AutoMod.Rules.Rules;

internal class CapsLockAutoModRule : AutoModRuleBase
{
    public override string RuleIdentifier => "CAPS";

    private const string CapsCountKey = "CAPS_LIMIT";
    private const int DefaultCapsLimit = 3;

    private const string WordsToIncrementLimitKey = "INCREMENT_COUNT";
    private const int DefaultIncrementCount = 5;


    public CapsLockAutoModRule(IAutoModBusinessLogic businessLogic) : base(businessLogic)
    {
    }

    protected override Dictionary<string, ConfigurationValueType> _keys { get; } = new()
    {
        { ValidationHelper.ActionKey, ConfigurationValueType.ActionValue },
        { CapsCountKey, ConfigurationValueType.IntValueOnly },
        { WordsToIncrementLimitKey, ConfigurationValueType.IntValueOnly },
    };


    public override IRuleViolationAction ExecuteRule(ICommandContext context)
    {
        if (!_guilds.Contains(context.Guild.Id))
        {
            return new DoNothingAction();
        }

        var allowedCapsCount = Configs[context.Guild.Id].GetValue(CapsCountKey).ToInt()
            .GetValueOrDefault(DefaultCapsLimit);
        var words = context.Message.Content.Split(' ', '\n');
        var capsCount = CalculateRelevantCapsCount(words);

        var noCapsCount = words.Length - capsCount;
        var wordsToIncrement = Configs[context.Guild.Id]
            .GetValue(WordsToIncrementLimitKey).ToInt().GetValueOrDefault(DefaultIncrementCount);
        var freeCapsCount = allowedCapsCount + noCapsCount / wordsToIncrement;
        if (capsCount < freeCapsCount)
        {
            return new DoNothingAction();
        }

        var onlyEmotesAction =
            ValidationHelper.MapValidation(Configs[context.Guild.Id].GetValue(ValidationHelper.ActionKey),
                "Zu viele Caps!");
        return onlyEmotesAction;
    }

    private int CalculateRelevantCapsCount(string[] words)
    {
        return words.Where(CountAsRelevantCaps).Count();
    }

    private bool CountAsRelevantCaps(string word)
    {
        if (word.Length < 3)
        {
            return false;
        }

        var charCount = word.Length;
        var capsCount = word.Where(char.IsUpper).Count();

        return capsCount > (charCount - capsCount) * 3;
    }
}