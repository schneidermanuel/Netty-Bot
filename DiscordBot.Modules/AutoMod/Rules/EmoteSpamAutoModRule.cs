using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBot.DataAccess.Contract.AutoMod;
using DiscordBot.DataAccess.Contract.AutoMod.Violation;
using DiscordBot.Framework.Extentions;

namespace DiscordBot.Modules.AutoMod.Rules;

internal class EmoteSpamAutoModRule : IGuildAutoModRule
{
    private readonly IAutoModBusinessLogic _businessLogic;
    private const string FreeEmoteKey = "FREE_EMOTES";
    private const int FreeEmoteDefaultCount = 5;
    public string RuleIdentifier => "EMOTE_SPAM";

    private IList<ulong> _guilds;
    private Dictionary<ulong, GuildRuleConfiguration> _configs;

    public EmoteSpamAutoModRule(IAutoModBusinessLogic businessLogic)
    {
        _businessLogic = businessLogic;
        _guilds = new List<ulong>();
        _configs = new Dictionary<ulong, GuildRuleConfiguration>();
    }

    public async Task InitializeAsync()
    {
        var guilds = await _businessLogic.GetGuildIdsWithModuleEnabled(RuleIdentifier);
        _guilds = guilds.ToList();
        foreach (var enabledGuild in _guilds)
        {
            var keyValuePairs = await _businessLogic.GetConfigurationsForGuildAndRule(enabledGuild, RuleIdentifier);
            foreach (var keyValuePair in keyValuePairs)
            {
                if (!_configs.ContainsKey(enabledGuild))
                {
                    _configs.Add(enabledGuild, new GuildRuleConfiguration());
                }

                _configs[enabledGuild].SetValue(keyValuePair.Key, keyValuePair.Value);
            }
        }
    }

    public IRuleViolationAction ExecuteRule(ICommandContext context)
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
            ValidationHelper.MapValidation(_configs[context.Guild.Id].GetValue(ValidationHelper.ActionKey),
                "Zu viele Emotes!");
        return onlyEmotesAction;
    }

    private bool DoesMessageViolateRule(ulong guildId, int wordCount, int emoteCount)
    {
        var config = _configs[guildId];

        var freeEmoteCount = config.GetValue(FreeEmoteKey).ToInt().GetValueOrDefault(FreeEmoteDefaultCount);
        if (emoteCount > freeEmoteCount && emoteCount > wordCount - wordCount)
        {
            return true;
        }

        return false;
    }

    public bool IsEnabledInGuild(ulong guildId)
    {
        return _guilds.Contains(guildId);
    }

    public void RegisterGuild(ulong guildId)
    {
        if (!_guilds.Contains(guildId))
        {
            _guilds.Add(guildId);
        }
    }

    public void UnregisterGuild(ulong guildId)
    {
        if (_guilds.Contains(guildId))
        {
            _guilds.Remove(guildId);
        }
    }

    public void SetValue(ulong guildId, string key, string value)
    {
        if (!_configs.ContainsKey(guildId))
        {
            _configs.Add(guildId, new GuildRuleConfiguration());
        }

        _configs[guildId].SetValue(key, value);
    }
}