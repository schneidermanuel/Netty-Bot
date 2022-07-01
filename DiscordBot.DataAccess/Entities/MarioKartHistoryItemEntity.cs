using System;

namespace DiscordBot.DataAccess.Entities;

public class MarioKartHistoryItemEntity
{
    public virtual long Id { get; set; }
    public virtual long MarioKartGameId { get; set; }
    public virtual int TeamPoints { get; set; }
    public virtual int EnemyPoints { get; set; }
    public virtual DateTime CreatedAt { get; set; }
    public virtual string Comment { get; set; }
}