using System;

namespace DiscordBot.DataAccess.Entities;

public class EventEntity
{
    public virtual long Id { get; set; }
    public virtual string GuildId { get; set; }
    public virtual string ChannelId { get; set; }
    public virtual string MessageId { get; set; }
    public virtual DateTime AutodeleteDate { get; set; }
    public virtual string RoleId { get; set; }
    public virtual int? MaxUsers { get; set; }
}