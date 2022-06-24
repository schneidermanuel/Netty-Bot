namespace DiscordBot.DataAccess.Entities;

internal class TwitterRegistrationEntity
{
    public virtual long RegistrationId { get; set; }
    public virtual string ChannelId { get; set; }
    public virtual string GuildId { get; set; }
    public virtual string TwitterUsername { get; set; }
    public virtual string Message { get; set; }
    public virtual string RuleFilter { get; set; }
}