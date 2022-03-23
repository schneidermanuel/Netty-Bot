using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.DataAccess.Contract.MusicPlayer
{
    public interface IMusicPlayerBusinessLogic
    {
        Task<bool> CanUserCreatePlaylistAsync(ulong userId);
        Task SavePlaylistAsync(Playlist playlist);
        Task<IEnumerable<Playlist>> RetrieveAllPlaylistsAsync();
        Task<Playlist> RetrieveSinglePlaylistAsync(long playlistId);
        Task DeletePlaylistAsync(long playlistId);
    }
}