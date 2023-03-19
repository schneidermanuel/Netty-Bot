namespace DiscordBot.DataAccess.Contract.TwitterRegistration;

public class TwitterRegistrationDto
{
    public long RegistrationId { get; set; }
    public ulong GuildId { get; set; }
    public ulong ChannelId { get; set; }
    public string Username { get; set; }
    public string Message { get; set; }
    public string RuleFilter { get; set; }
}