namespace DiscordBot.DataAccess.Entities;

public class MarioKartRunnningGameEntity
{
    public virtual string UserId { get; set; }
    public virtual long GameId { get; set; }
    public virtual int TeamPoints { get; set; }
    public virtual int EnemyPoints { get; set; }
}