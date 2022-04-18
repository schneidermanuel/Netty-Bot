using System;

namespace DiscordBot.DataAccess.Modules.GeburtstagList.BusinessLogic;

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