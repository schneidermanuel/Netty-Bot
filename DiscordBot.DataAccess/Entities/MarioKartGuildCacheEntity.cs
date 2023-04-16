namespace DiscordBot.DataAccess.Entities;

public class MarioKartGuildCacheEntity 
{
    public virtual long Id { get; set; }
    public virtual string GuildId { get; set; }
    public virtual string TeamName { get; set; }
    public virtual string TeamImage { get; set; }
    public virtual string EnemyName { get; set; }
    public virtual string EnemyImage { get; set; }
}