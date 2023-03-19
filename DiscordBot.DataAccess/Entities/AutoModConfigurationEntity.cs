namespace DiscordBot.DataAccess.Entities;

public class AutoModConfigurationEntity
{
    public virtual long ConfigurationId { get; set; }
    public virtual string GuildId { get; set; }
    public virtual string RuleKey { get; set; }
    public virtual string ConfigurationKey { get; set; }
    public virtual string ConfigurationValue { get; set; }
}