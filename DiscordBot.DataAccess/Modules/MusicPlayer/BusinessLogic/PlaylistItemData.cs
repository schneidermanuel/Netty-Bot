namespace DiscordBot.DataAccess.Modules.MusicPlayer.BusinessLogic;

internal class PlaylistItemData
{
    public PlaylistItemData(long playlistItemId, string url, long playlistId)
    {
        PlaylistItemId = playlistItemId;
        Url = url;
        PlaylistId = playlistId;
    }

    public long PlaylistItemId { get; }
    public string Url { get; }
    public long PlaylistId { get; }
}