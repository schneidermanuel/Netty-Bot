using System.Collections.Generic;
using Discord.Commands;
using DiscordBot.DataAccess.Contract.AutoMod;
using DiscordBot.DataAccess.Contract.AutoMod.Violation;
using DiscordBot.Framework.Extentions;

namespace DiscordBot.Modules.AutoMod.Rules.Rules;

internal class SpamAutoModRule : AutoModRuleBase
{
    private const string TimeKey = "RESET_TIME";
    private const int DefaultTime = 5;
    private const string MsgInTime = "MESSAGE_COUNT";
    private const int DefaultMessageCount = 3;

    private Dictionary<ulong, GuildMessageObservation> _guildObservations;

    public SpamAutoModRule(IAutoModDomain domain) : base(domain)
    {
        _guildObservations = new Dictionary<ulong, GuildMessageObservation>();
    }

    protected override Dictionary<string, ConfigurationValueType> _keys { get; } = new()
    {
        { ValidationHelper.ActionKey, ConfigurationValueType.ActionValue },
        { TimeKey, ConfigurationValueType.IntValueOnly },
        { MsgInTime, ConfigurationValueType.IntValueOnly }
    };

    public override string RuleIdentifier => "SPAM";

    public override IRuleViolationAction ExecuteRule(ICommandContext context)
    {
        var guildId = context.Guild.Id;
        if (!Guilds.Contains(guildId))
        {
            return new DoNothingAction();
        }

        var observation = GetGuildMessageObservation(guildId);
        var messageCount = observation.RegisterMessage(context.User.Id,
            GetConfig(guildId, TimeKey).ToInt().GetValueOrDefault(DefaultTime));

        var maxMessageCount = GetConfig(guildId, MsgInTime).ToInt().GetValueOrDefault(DefaultMessageCount);
        if (messageCount >= maxMessageCount)
        {
            return ValidationHelper.MapValidation(GetConfig(context.Guild.Id, ValidationHelper.ActionKey), nameof(AutoModRessources.Validation_Spamming));
        }

        return new DoNothingAction();
    }

    private GuildMessageObservation GetGuildMessageObservation(ulong guildId)
    {
        if (!_guildObservations.ContainsKey(guildId))
        {
            _guildObservations.Add(guildId, new GuildMessageObservation());
        }

        return _guildObservations[guildId];
    }
}