namespace DiscordBot.DataAccess.Modules.GeburtstagList.Domain;

internal class BirthdaySubChannelData
{
    public BirthdaySubChannelData(long id, string guildId, string channelId)
    {
        Id = id;
        GuildId = guildId;
        ChannelId = channelId;
    }

    public BirthdaySubChannelData(long id, ulong guildId, ulong channelId)
    {
        Id = id;
        GuildId = guildId.ToString();
        ChannelId = channelId.ToString();
    }


    public long Id { get; }
    public string GuildId { get; }
    public string ChannelId { get; }
}