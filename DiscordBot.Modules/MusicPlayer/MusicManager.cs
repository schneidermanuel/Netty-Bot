using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract.MusicPlayer;
using DiscordBot.MusicPlayer;
using Victoria;
using Victoria.Enums;
using Victoria.EventArgs;

namespace DiscordBot.Modules.MusicPlayer;

public class MusicManager
{
    private readonly LavaNode _node;
    private readonly PlaylistManager _playlistManager;

    public MusicManager(LavaNode node, PlaylistManager playlistManager)
    {
        _node = node;
        _playlistManager = playlistManager;
    }


    public async Task Initialize()
    {
        if (_node.IsConnected)
        {
            Console.WriteLine("Already Connected");
            return;
        }

        Console.WriteLine("Connecting to Lavalink...");
        await _node.ConnectAsync();
        Console.WriteLine("LavaLink Connection established!");
        _node.OnTrackEnded += TrackEnded;
    }

    public async Task PlayTrackAsync(IGuild guild, LavaTrack track, IVoiceChannel voiceChannel)
    {
        if (!_node.IsConnected)
        {
            await _node.ConnectAsync();
        }

        _playlistManager.AddSongToPlaylist(guild.Id, track);
        if (!_node.HasPlayer(guild))
        {
            await _node.JoinAsync(voiceChannel);
        }

        var player = _node.GetPlayer(guild);
        if (!player.IsConnected)
        {
            player = await _node.JoinAsync(voiceChannel);
        }

        if (player.PlayerState != PlayerState.Playing)
        {
            await player.PlayAsync(_playlistManager.GetNextSong(guild.Id));
        }
    }

    private async Task TrackEnded(TrackEndedEventArgs arg)
    {
        if (arg.Reason != TrackEndReason.Finished)
        {
            return;
        }

        var guild = arg.Player.VoiceChannel.Guild;
        await PlayNextSongAsync(guild);
    }

    private async Task PlayNextSongAsync(IGuild guild)
    {
        var nextSong = _playlistManager.GetNextSong(guild.Id);
        if (nextSong == null)
        {
            return;
        }

        if (!_node.HasPlayer(guild))
        {
            return;
        }


        var player = _node.GetPlayer(guild);
        var channel = (SocketVoiceChannel)player.VoiceChannel;
        if (channel.Users.Count < 2)
        {
            _playlistManager.DeletePlaylist(guild.Id);
            await player.VoiceChannel.DisconnectAsync();
            await player.DisposeAsync();
        }

        await player.PlayAsync(nextSong);
    }

    public async Task SkipTrack(IGuild guild)
    {
        await PlayNextSongAsync(guild);
    }

    public string RetrieveSongs(IGuild guild, int skipPages)
    {
        return _playlistManager.RetrieveQueue(guild.Id, skipPages);
    }

    public async Task ClearQueueAsync(IGuild guild)
    {
        _playlistManager.DeletePlaylist(guild.Id);
        var player = _node.GetPlayer(guild);
        if (player.VoiceChannel == null)
        {
            await player.DisposeAsync();
            return;
        }

        await _node.LeaveAsync(player.VoiceChannel);
    }

    public async Task PauseAsync(IGuild guild)
    {
        if (_node.HasPlayer(guild))
        {
            var player = _node.GetPlayer(guild);
            await player.PauseAsync();
        }
    }

    public async Task ResumeAsync(IGuild guild)
    {
        if (_node.HasPlayer(guild))
        {
            var player = _node.GetPlayer(guild);
            await player.ResumeAsync();
        }
    }

    public Playlist CreatePlaylist(string name, ulong guildId, ulong userId)
    {
        var tracks = _playlistManager.GetTracksAsPlaylistItems(guildId);
        var playlist = new Playlist
        {
            Title = name,
            AuthorId = userId,
            Tracks = tracks
        };
        return playlist;
    }

    public void ShufflePlaylist(IGuild contextGuild)
    {
        _playlistManager.ShufflePlaylist(contextGuild.Id);
    }

    public int GetSongCount(IGuild contextGuild)
    {
        return _playlistManager.GetCountForGuildId(contextGuild.Id);
    }

}