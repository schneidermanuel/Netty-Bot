using System;

namespace DiscordBot.DataAccess.Contract.GeburtstagList
{
    public class Birthday
    {
        public ulong UserId { get; set; }
        public DateTime Geburtsdatum { get; set; }
    }
}