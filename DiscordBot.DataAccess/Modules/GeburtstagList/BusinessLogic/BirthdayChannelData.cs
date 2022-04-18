namespace DiscordBot.DataAccess.Modules.GeburtstagList.BusinessLogic;

internal class BirthdayChannelData
{
    public BirthdayChannelData(
        long id,
        string guildId,
        string channelId,
        string janMessageId,
        string febMessageId,
        string marMessageId,
        string aprMessageId,
        string maiMessageId,
        string junMessageId,
        string julMessageId,
        string augMessageId,
        string sepMessageId,
        string octMessageId,
        string novMessageId,
        string dezMessageId)
    {
        Id = id;
        GuildId = guildId;
        ChannelId = channelId;
        JanMessageId = janMessageId;
        FebMessageId = febMessageId;
        MarMessageId = marMessageId;
        AprMessageId = aprMessageId;
        MaiMessageId = maiMessageId;
        JunMessageId = junMessageId;
        JulMessageId = julMessageId;
        AugMessageId = augMessageId;
        SepMessageId = sepMessageId;
        OctMessageId = octMessageId;
        NovMessageId = novMessageId;
        DezMessageId = dezMessageId;
    }

    public BirthdayChannelData(
        long id,
        ulong guildId,
        ulong channelId,
        ulong janMessageId,
        ulong febMessageId,
        ulong marMessageId,
        ulong aprMessageId,
        ulong maiMessageId,
        ulong junMessageId,
        ulong julMessageId,
        ulong augMessageId,
        ulong sepMessageId,
        ulong octMessageId,
        ulong novMessageId,
        ulong dezMessageId)
    {
        Id = id;
        GuildId = guildId.ToString();
        ChannelId = channelId.ToString();
        JanMessageId = janMessageId.ToString();
        FebMessageId = febMessageId.ToString();
        MarMessageId = marMessageId.ToString();
        AprMessageId = aprMessageId.ToString();
        MaiMessageId = maiMessageId.ToString();
        JunMessageId = junMessageId.ToString();
        JulMessageId = julMessageId.ToString();
        AugMessageId = augMessageId.ToString();
        SepMessageId = sepMessageId.ToString();
        OctMessageId = octMessageId.ToString();
        NovMessageId = novMessageId.ToString();
        DezMessageId = dezMessageId.ToString();
    }


    public long Id { get; }
    public string GuildId { get; }
    public string ChannelId { get; }
    public string JanMessageId { get; }
    public string FebMessageId { get; }
    public string MarMessageId { get; }
    public string AprMessageId { get; }
    public string MaiMessageId { get; }
    public string JunMessageId { get; }
    public string JulMessageId { get; }
    public string AugMessageId { get; }
    public string SepMessageId { get; }
    public string OctMessageId { get; }
    public string NovMessageId { get; }
    public string DezMessageId { get; }
}