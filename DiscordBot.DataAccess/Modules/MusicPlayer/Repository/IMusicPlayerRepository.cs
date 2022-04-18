using System.Collections.Generic;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Modules.MusicPlayer.BusinessLogic;

namespace DiscordBot.DataAccess.Modules.MusicPlayer.Repository;

internal interface IMusicPlayerRepository
{
    Task<bool> CanUserCreatePlaylistAsync(string userId);
    Task<long> SavePlaylistAsync(PlaylistData playlistData);
    Task SaveTrackAsync(PlaylistItemData track);
    Task<IEnumerable<PlaylistData>> RetrieveAllPLaylistsAsync();
    Task<IEnumerable<PlaylistItemData>> RetrieveTracksForPlaylistAsync(PlaylistData playlistData);
    Task<PlaylistData> RetrievePlaylistDataAsync(long playlistId);
    Task DeletePlaylistAsync(long playlistId);
}