namespace DiscordBot.DataAccess.Entities;

public class BirthdayRoleAssotiationEntity
{
    public virtual long AssotiationId { get; set; }
    public virtual string GuildId { get; set; }
    public virtual string UserId { get; set; }
}