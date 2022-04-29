using System.Collections.Generic;
using System.Linq;
using Discord;
using Discord.Commands;
using DiscordBot.DataAccess.Contract.AutoMod;
using DiscordBot.DataAccess.Contract.AutoMod.Violation;
using DiscordBot.Framework.Extentions;

namespace DiscordBot.Modules.AutoMod.Rules.Rules;

internal class EmoteSpamAutoModRule : AutoModRuleBase
{
    public override string RuleIdentifier => "EMOTE_SPAM";

    private const int FreeEmoteDefaultCount = 5;
    private const string FreeEmoteKey = "FREE_EMOTES";
    
    protected override Dictionary<string, ConfigurationValueType> _keys { get; } = new()
    {
        { FreeEmoteKey, ConfigurationValueType.IntValueOnly },
        { ValidationHelper.ActionKey, ConfigurationValueType.ActionValue }
    };

    public EmoteSpamAutoModRule(IAutoModBusinessLogic businessLogic) : base(businessLogic)
    {
    }


    public override IRuleViolationAction ExecuteRule(ICommandContext context)
    {
        if (!_guilds.Contains(context.Guild.Id))
        {
            return new DoNothingAction();
        }

        var words = context.Message.Content.Split(' ', '\n');
        var emojiCount = context.Message.Content.Count(c => c > 3000) / 2;
        var emoteCount = words.Count(word => Emote.TryParse(word, out _));
        var violation = DoesMessageViolateRule(context.Guild.Id, words.Count(), emojiCount + emoteCount);
        if (!violation)
        {
            return new DoNothingAction();
        }

        var onlyEmotesAction =
            ValidationHelper.MapValidation(Configs[context.Guild.Id].GetValue(ValidationHelper.ActionKey),
                "Zu viele Emotes!");
        return onlyEmotesAction;
    }

    private bool DoesMessageViolateRule(ulong guildId, int wordCount, int emoteCount)
    {
        var config = Configs[guildId];

        var freeEmoteCount = config.GetValue(FreeEmoteKey).ToInt().GetValueOrDefault(FreeEmoteDefaultCount);
        if (emoteCount > freeEmoteCount && emoteCount > wordCount - wordCount)
        {
            return true;
        }

        return false;
    }
}