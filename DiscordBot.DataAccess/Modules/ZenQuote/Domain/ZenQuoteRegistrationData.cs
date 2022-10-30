namespace DiscordBot.DataAccess.Modules.ZenQuote.Domain;

internal class ZenQuoteRegistrationData
{
    public ZenQuoteRegistrationData(long id, string guildId, string channelId)
    {
        Id = id;
        GuildId = guildId;
        ChannelId = channelId;
    }
    public ZenQuoteRegistrationData(long id, ulong guildId, ulong channelId)
    {
        Id = id;
        GuildId = guildId.ToString();
        ChannelId = channelId.ToString();
    }

    public long Id { get;  }
    public string GuildId { get;  }
    public string ChannelId { get;  }
}