namespace DiscordBot.DataAccess.Modules.UserConfiguration.Domain;

internal class UserConfigurationData
{
    public long Id { get; }
    public string UserId { get; }
    public string LanguageCode { get; }

    public UserConfigurationData(long id, string userId, string languageCode)
    {
        Id = id;
        UserId = userId;
        LanguageCode = languageCode;
    }

    public UserConfigurationData(long id, ulong userId, string languageCode)
    {
        Id = id;
        LanguageCode = languageCode;
        UserId = userId.ToString();
    }
    
    
}