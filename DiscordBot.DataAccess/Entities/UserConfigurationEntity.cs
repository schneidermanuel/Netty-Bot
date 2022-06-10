namespace DiscordBot.DataAccess.Entities;

public class UserConfigurationEntity
{
    public virtual long Id { get; set; }
    public virtual string UserId { get; set; }
    public virtual string LanguageCode { get; set; }
}