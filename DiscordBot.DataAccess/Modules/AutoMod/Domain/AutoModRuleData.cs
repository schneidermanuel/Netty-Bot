namespace DiscordBot.DataAccess.Modules.AutoMod.Domain;

internal class AutoModRuleData
{
    public string RuleKey { get; }
    public string ConfigKey { get; }
    public string Value { get; }

    public AutoModRuleData(string ruleKey, string configKey, string value)
    {
        RuleKey = ruleKey;
        ConfigKey = configKey;
        Value = value;
    }
}