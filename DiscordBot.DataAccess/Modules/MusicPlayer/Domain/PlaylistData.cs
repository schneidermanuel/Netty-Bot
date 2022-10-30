namespace DiscordBot.DataAccess.Modules.MusicPlayer.Domain;

internal class PlaylistData
{
    public PlaylistData(long playlistId, string userId, string title)
    {
        PlaylistId = playlistId;
        UserId = userId;
        Title = title;
    }

    public PlaylistData(long playlistId, ulong userId, string title)
    {
        PlaylistId = playlistId;
        UserId = userId.ToString();
        Title = title;
    }


    public long PlaylistId { get; }
    public string UserId { get; }
    public string Title { get; }
}