using System.Collections.Generic;

namespace DiscordBot.DataAccess.Contract.MusicPlayer
{
    public class Playlist
    {
        public long PlaylistId { get; set; }
        public ulong AuthorId { get; set; }
        public string Title { get; set; }
        public List<PlaylistItem> Tracks { get; set; }
    }
}