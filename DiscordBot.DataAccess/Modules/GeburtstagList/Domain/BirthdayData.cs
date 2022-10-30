using System;

namespace DiscordBot.DataAccess.Modules.GeburtstagList.Domain;

internal class BirthdayData
{
    public BirthdayData(string userId, DateTime birthday)
    {
        UserId = userId;
        Birthday = birthday;
    }

    public BirthdayData(ulong userId, DateTime birthday)
    {
        UserId = userId.ToString();
        Birthday = birthday;
    }

    public string UserId { get; }

    public DateTime Birthday { get; }
}