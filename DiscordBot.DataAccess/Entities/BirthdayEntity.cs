using System;

namespace DiscordBot.DataAccess.Entities;

public class BirthdayEntity
{
    public virtual string UserId { get; set; }

    public virtual DateTime Birthday { get; set; }
}