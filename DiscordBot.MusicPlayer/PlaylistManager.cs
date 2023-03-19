using System.Collections.Generic;
using System.Linq;
using DiscordBot.DataAccess.Contract.MusicPlayer;
using Victoria;

namespace DiscordBot.MusicPlayer;

public class PlaylistManager
{
    private readonly Dictionary<ulong, List<LavaTrack>> _playlists = new();

    public LavaTrack GetNextSong(ulong guildId)
    {
        if (!_playlists.ContainsKey(guildId)) return null;
        var playlist = _playlists[guildId];
        if (!playlist.Any())
        {
            return null;
        }

        var currentSong = playlist.First();
        playlist.RemoveAt(0);
        playlist.Add(currentSong);
        return playlist.First();
    }

    public void AddSongToPlaylist(ulong guildId, LavaTrack song)
    {
        if (_playlists.ContainsKey(guildId))
        {
            var playlist = _playlists[guildId];
            playlist.Add(song);
            return;
        }

        _playlists.Add(guildId, new List<LavaTrack> { song });
    }

    public void DeletePlaylist(ulong guildId)
    {
        if (_playlists.ContainsKey(guildId))
        {
            _playlists.Remove(guildId);
        }
    }

    public string RetrieveQueue(ulong guild, int skipPages)
    {
        if (!_playlists.ContainsKey(guild))
        {
            return null;
        }

        var playlist = _playlists[guild];
        var tracks = playlist.Skip(skipPages * 10).Take(10);
        return
            tracks.Aggregate(string.Empty,
                (current, track) =>
                    current +
                    $"{track.Author} - {track.Title} ({track.Duration.Minutes}:{track.Duration.Seconds})\n");
    }

    public List<PlaylistItem> GetTracksAsPlaylistItems(ulong guildId)
    {
        if (!_playlists.ContainsKey(guildId))
        {
            return null;
        }

        var list = _playlists[guildId].Select(track => new PlaylistItem { Title = $"{track.Author} {track.Title}" })
            .ToList();
        return list;
    }

    public void ShufflePlaylist(ulong guildId)
    {
        if (!_playlists.ContainsKey(guildId))
        {
            return;
        }

        _playlists[guildId].Shuffle();
    }

    public int GetCountForGuildId(ulong guildId)
    {
        if (!_playlists.ContainsKey(guildId))
        {
            return 0;
        }

        return _playlists[guildId].Count;
    }
}