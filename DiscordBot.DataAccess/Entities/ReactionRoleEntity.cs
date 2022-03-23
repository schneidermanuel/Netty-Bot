namespace DiscordBot.DataAccess.Entities;

public class ReactionRoleEntity
{
    public virtual long ReactionRoleId { get; set; }
    public virtual string GuildId { get; set; }
    public virtual string ChannelId { get; set; }
    public virtual string MessageId { get; set; }
    public virtual string EmojiId { get; set; }
    public virtual string RoleId { get; set; }
    public virtual bool IsEmoji { get; set; }
}