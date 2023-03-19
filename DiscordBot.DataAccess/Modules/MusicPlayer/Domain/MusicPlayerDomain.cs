using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Contract.MusicPlayer;
using DiscordBot.DataAccess.Modules.MusicPlayer.Repository;

namespace DiscordBot.DataAccess.Modules.MusicPlayer.Domain;

internal class MusicPlayerDomain : IMusicPlayerDomain
{
    private readonly IMusicPlayerRepository _repository;

    public MusicPlayerDomain(IMusicPlayerRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> CanUserCreatePlaylistAsync(ulong userId)
    {
        return await _repository.CanUserCreatePlaylistAsync(userId.ToString());
    }

    public async Task SavePlaylistAsync(Playlist playlist)
    {
        var playlistData = new PlaylistData(playlist.PlaylistId, playlist.AuthorId, playlist.Title);
        var playlistId = await _repository.SavePlaylistAsync(playlistData);
        var tracks = playlist.Tracks.Select(track =>
                new PlaylistItemData(track.PlaylistItemId, track.Title, playlistId))
            .ToList();
        foreach (var track in tracks)
        {
            await _repository.SaveTrackAsync(track);
        }
    }

    public async Task<IEnumerable<Playlist>> RetrieveAllPlaylistsAsync()
    {
        var playlistDatas = await _repository.RetrieveAllPLaylistsAsync();
        var playlists = new List<Playlist>();
        foreach (var playlistData in playlistDatas)
        {
            playlists.Add(await EnrichPlaylistDataToDto(playlistData));
        }

        return playlists;
    }

    public async Task<Playlist> RetrieveSinglePlaylistAsync(long playlistId)
    {
        var playlistData = await _repository.RetrievePlaylistDataAsync(playlistId);
        return playlistData == null ? null : await EnrichPlaylistDataToDto(playlistData);
    }

    public async Task DeletePlaylistAsync(long playlistId)
    {
        await _repository.DeletePlaylistAsync(playlistId);
    }

    private async Task<Playlist> EnrichPlaylistDataToDto(PlaylistData playlistData)
    {
        var tracks = await _repository.RetrieveTracksForPlaylistAsync(playlistData);
        return new Playlist
        {
            Title = playlistData.Title,
            AuthorId = ulong.Parse(playlistData.UserId),
            PlaylistId = playlistData.PlaylistId,
            Tracks = tracks.Select(MapTrackToDto).ToList()
        };
    }

    private PlaylistItem MapTrackToDto(PlaylistItemData data)
    {
        return new PlaylistItem
        {
            Title = data.Url,
            PlaylistItemId = data.PlaylistItemId
        };
    }
}