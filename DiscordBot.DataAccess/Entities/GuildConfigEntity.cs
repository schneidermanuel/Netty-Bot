namespace DiscordBot.DataAccess.Entities;

public class GuildConfigEntity
{
    public virtual long Id { get; set; }
    public virtual string GuildId { get; set; }
    public virtual string Prefix { get; set; }
}