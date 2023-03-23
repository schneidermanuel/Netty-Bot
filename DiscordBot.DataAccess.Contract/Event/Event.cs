using System;

namespace DiscordBot.DataAccess.Contract.Event;

public class Event
{
    public long EventId { get; set; }
    public ulong GuildId { get; set; }
    public int? MaxUsers { get; set; }
    public DateTime AutoDeleteDate { get; set; }
    public ulong? RoleId { get; set; }
    public ulong OwnerUserId { get; set; }
}