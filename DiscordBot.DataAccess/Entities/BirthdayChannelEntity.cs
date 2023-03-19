namespace DiscordBot.DataAccess.Entities;

public class BirthdayChannelEntity
{
    public virtual long Id { get; set; }
    public virtual string GuildId { get; set; }
    public virtual string ChannelId { get; set; }
    public virtual string JanMessageId { get; set; }
    public virtual string FebMessageId { get; set; }
    public virtual string MarMessageId { get; set; }
    public virtual string AprMessageId { get; set; }
    public virtual string MaiMessageId { get; set; }
    public virtual string JunMessageId { get; set; }
    public virtual string JulMessageId { get; set; }
    public virtual string AugMessageId { get; set; }
    public virtual string SepMessageId { get; set; }
    public virtual string OctMessageId { get; set; }
    public virtual string NovMessageId { get; set; }
    public virtual string DezMessageId { get; set; }
}