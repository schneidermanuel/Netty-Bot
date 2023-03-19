namespace DiscordBot.DataAccess.Entities;

public class PlaylistEntity
{
    public virtual long PlaylistId { get; set; }
    public virtual string UserId { get; set; }
    public virtual string Title { get; set; }
}