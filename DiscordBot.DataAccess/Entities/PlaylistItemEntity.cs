namespace DiscordBot.DataAccess.Entities;

public class PlaylistItemEntity
{
    public virtual long PlaylistItemId { get; set; }
    public virtual string Url { get; set; }
    public virtual long PlaylistId { get; set; }
}