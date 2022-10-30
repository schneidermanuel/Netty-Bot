namespace DiscordBot.DataAccess.Modules.AutoMod.Domain;

internal class AutoModConfigurationData
{
    public AutoModConfigurationData(
        long configurationId, 
        string guildId, 
        string ruleKey, 
        string configurationKey, 
        string configurationValue)
    {
        ConfigurationId = configurationId;
        GuildId = guildId;
        RuleKey = ruleKey;
        ConfigurationKey = configurationKey;
        ConfigurationValue = configurationValue;
    }
    public AutoModConfigurationData(
        long configurationId, 
        ulong guildId, 
        string ruleKey, 
        string configurationKey, 
        string configurationValue)
    {
        ConfigurationId = configurationId;
        GuildId = guildId.ToString();
        RuleKey = ruleKey;
        ConfigurationKey = configurationKey;
        ConfigurationValue = configurationValue;
    }

    public long ConfigurationId { get; }
    public string GuildId { get; }
    public string RuleKey { get; }
    public string ConfigurationKey { get; }
    public string ConfigurationValue { get; }
}