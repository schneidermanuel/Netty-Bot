using System;

namespace DiscordBot.DataAccess.Modules.GeburtstagList.BusinessLogic;

public class BirthdayData
{
    public ulong UserId { get; set; }
    public DateTime Geburtsdatum { get; set; }
}