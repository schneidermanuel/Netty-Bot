namespace DiscordBot.DataAccess.Entities;

public class MarioKartRunnningGameEntity
{
    public virtual string ChannelId { get; set; }
    public virtual long GameId { get; set; }
    public virtual int TeamPoints { get; set; }
    public virtual int EnemyPoints { get; set; }
    public virtual string GuildId { get; set; }
    public virtual bool IsCompleted { get; set; }
    public virtual string GameName { get; set; }
}