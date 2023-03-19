namespace DiscordBot.DataAccess.Modules.TwitterRegistration.Domain;

internal class TwitterRegistrationData
{
    public long RegistrationId { get; set; }
    public string GuildId { get; set; }
    public string ChannelId { get; set; }
    public string TwitterUsername { get; set; }
    public string Message { get; }
    public string RuleFilter { get; }

    public TwitterRegistrationData(long registrationId, string guildId, string channelId, string twitterUsername, string message, string ruleFilter)
    {
        RegistrationId = registrationId;
        GuildId = guildId;
        ChannelId = channelId;
        TwitterUsername = twitterUsername;
        Message = message;
        RuleFilter = ruleFilter;
    }
    
    public TwitterRegistrationData(long registrationId, ulong guildId, ulong channelId, string twitterUsername, string message, string ruleFilter)
    {
        RegistrationId = registrationId;
        GuildId = guildId.ToString();
        ChannelId = channelId.ToString();
        TwitterUsername = twitterUsername;
        Message = message;
        RuleFilter = ruleFilter;
    }

}