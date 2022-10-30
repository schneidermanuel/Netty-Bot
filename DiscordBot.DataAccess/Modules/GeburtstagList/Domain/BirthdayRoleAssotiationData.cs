namespace DiscordBot.DataAccess.Modules.GeburtstagList.Domain;

internal class BirthdayRoleAssotiationData
{
    public BirthdayRoleAssotiationData(long assotiationId, string guildId, string userId)
    {
        AssotiationId = assotiationId;
        GuildId = guildId;
        UserId = userId;
    }
    public BirthdayRoleAssotiationData(long assotiationId, ulong guildId, ulong userId)
    {
        AssotiationId = assotiationId;
        GuildId = guildId.ToString();
        UserId = userId.ToString();
    }

    public long AssotiationId { get; }
    public string GuildId { get; }
    public string UserId { get; }
}