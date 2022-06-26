using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBot.DataAccess.Contract;
using DiscordBot.DataAccess.Contract.MusicPlayer;
using DiscordBot.Framework.Contract.Modularity;
using Microsoft.VisualBasic;
using Victoria;

namespace DiscordBot.Modules.MusicPlayer;

internal class MusicPlayerCommands : CommandModuleBase, IGuildModule
{
    private readonly LavaNode _lavaNode;
    private readonly MusicManager _manager;
    private readonly IMusicPlayerBusinessLogic _businessLogic;
    private readonly SpotifyApiManager _spotifyApiManager;

    public MusicPlayerCommands(IModuleDataAccess dataAccess, LavaNode lavaNode, MusicManager manager,
        IMusicPlayerBusinessLogic businessLogic, SpotifyApiManager spotifyApiManager) :
        base(dataAccess)
    {
        _lavaNode = lavaNode;
        _manager = manager;
        _businessLogic = businessLogic;
        _spotifyApiManager = spotifyApiManager;
    }

    protected override Type RessourceType => typeof(MusicPlayerRessources);
    public override string ModuleUniqueIdentifier => "MUSIC_PLAYER";

    public override async Task<bool> CanExecuteAsync(ulong id, SocketCommandContext socketCommandContext)
    {
        return await IsEnabled(id);
    }

    public override async Task ExecuteAsync(ICommandContext context)
    {
        await ExecuteCommandsAsync(context);
    }

    [Command("play")]
    public async Task PlayCommand(ICommandContext context)
    {
        try
        {
            var voiceState = context.User as IVoiceState;
            var songname = await RequireReminderArg(context);
            if (!await PlaySongAsync(songname, context.Channel, context.Guild, voiceState)) return;
            await context.Message.AddReactionAsync(Emoji.Parse("🤝"));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    [Command("loadSpotify")]
    public async Task LoadSpotifyAsync(ICommandContext context)
    {
        var voiceState = context.User as IVoiceState;
        if (voiceState?.VoiceChannel == null)
        {
            await context.Channel.SendMessageAsync(Localize(nameof(MusicPlayerRessources.Error_MustBeInVoice)));
            return;
        }

        var playlistUrl = await RequireString(context);
        try
        {
            var playlistId = playlistUrl.Split("https://open.spotify.com/playlist/")[1].Split('?').First();
            var playlist = await _spotifyApiManager.GetPlaylistInformationAsync(playlistId);
            await context.Channel.SendMessageAsync(string.Format(
                Localize(nameof(MusicPlayerRessources.Status_PlaylistLoading)), playlist.Name, playlist.TrackCount));
            var tracks = await _spotifyApiManager.GetPlaylistAsync(playlistId);

            await PlaySongAsync(tracks.First(), context.Channel, context.Guild, voiceState);

            var results = await Task.WhenAll(tracks.Skip(1)
                .Select(track => PlaySongAsync(track, context.Channel, context.Guild, voiceState)));

            var count = results.Count(result => result);

            await context.Channel.SendMessageAsync(string.Format(
                Localize(nameof(MusicPlayerRessources.Message_PlaylistLoaded)), playlist.Name, count + 1,
                playlist.TrackCount));
        }
        catch (Exception)
        {
            await context.Channel.SendMessageAsync(
                Localize(nameof(MusicPlayerRessources.Error_SpotifyPlaylistNotParsable)));
        }
    }

    private async Task<bool> PlaySongAsync(string songname, IMessageChannel messageChannel, IGuild guild,
        IVoiceState voiceState)
    {
        var searchSongTask = _lavaNode.SearchYouTubeAsync(songname);
        if (voiceState?.VoiceChannel == null)
        {
            await messageChannel.SendMessageAsync(Localize(nameof(MusicPlayerRessources.Error_MustBeInVoice)));
            return false;
        }

        var channel = voiceState.VoiceChannel;

        var track = await searchSongTask;
        if (!track.Tracks.Any())
        {
            await messageChannel.SendMessageAsync(
                string.Format(Localize(nameof(MusicPlayerRessources.Error_SongNotFound)), songname));
            return false;
        }

        await _manager.PlayTrackAsync(guild, track.Tracks.First(), channel);

        return true;
    }


    [Command("skip")]
    public async Task SkipCommand(ICommandContext context)
    {
        if (!_lavaNode.HasPlayer(context.Guild))
        {
            await context.Channel.SendMessageAsync(Localize(nameof(MusicPlayerRessources.Error_NoMusic)));
            return;
        }

        await _manager.SkipTrack(context.Guild);
        await context.Message.AddReactionAsync(Emoji.Parse("🤝"));
    }

    [Command("queue")]
    public async Task Queue(ICommandContext context)
    {
        var skipPages = RequireIntArgOrDefault(context, 1, 1) - 1;
        var songCount = _manager.GetSongCount(context.Guild);
        if (songCount == 0)
        {
            await context.Channel.SendMessageAsync(Localize(nameof(MusicPlayerRessources.Error_NoMusic)));
            return;
        }

        var pageCount = (songCount / 10) + 1;
        if (skipPages > pageCount)
        {
            await context.Channel.SendMessageAsync(Localize(nameof(MusicPlayerRessources.Error_InvlaidPage)));
            return;
        }

        var songs = _manager.RetrieveSongs(context.Guild, skipPages);
        var embedBuilder = new EmbedBuilder();
        embedBuilder.WithColor(Color.Blue);
        embedBuilder.WithDescription(songs);
        embedBuilder.WithCurrentTimestamp();
        embedBuilder.WithTitle(string.Format(Localize(nameof(MusicPlayerRessources.Title_QueueTitle)), songCount,
            skipPages + 1, pageCount));
        await context.Channel.SendMessageAsync("", false, embedBuilder.Build());
    }

    [Command("stop")]
    public async Task ClearCommand(ICommandContext context)
    {
        try
        {
            if (!_lavaNode.HasPlayer(context.Guild))
            {
                await context.Channel.SendMessageAsync(Localize(nameof(MusicPlayerRessources.Error_NoMusic)));
                return;
            }

            try
            {
                await _lavaNode.GetPlayer(context.Guild).StopAsync();
            }
            catch (Exception)
            {
                //Ignored
            }

            await _manager.ClearQueueAsync(context.Guild);

            await context.Message.AddReactionAsync(Emoji.Parse("🤝"));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Console.WriteLine(e.StackTrace);
        }
    }

    [Command("pause")]
    public async Task PauseCommand(ICommandContext context)
    {
        if (!_lavaNode.HasPlayer(context.Guild))
        {
            await context.Channel.SendMessageAsync(Localize(nameof(MusicPlayerRessources.Error_NoMusic)));
            return;
        }

        await _manager.PauseAsync(context.Guild);
        await context.Message.AddReactionAsync(Emoji.Parse("🤝"));
    }

    [Command("resume")]
    public async Task ResumeCommand(ICommandContext context)
    {
        if (!_lavaNode.HasPlayer(context.Guild))
        {
            await context.Channel.SendMessageAsync(Localize(nameof(MusicPlayerRessources.Error_NoMusic)));
            return;
        }

        await _manager.ResumeAsync(context.Guild);
        await context.Message.AddReactionAsync(Emoji.Parse("🤝"));
    }

    [Command("createPlaylist")]
    public async Task SaveQueueToDatabaseAsync(ICommandContext context)
    {
        var userId = context.User.Id;
        var canUserCreatePlaylist = await _businessLogic.CanUserCreatePlaylistAsync(userId);
        if (!canUserCreatePlaylist)
        {
            await context.Channel.SendMessageAsync(Localize(nameof(MusicPlayerRessources.Error_TooManyPlaylists)));
            return;
        }

        var title = await RequireReminderArg(context);
        var playlist = _manager.CreatePlaylist(title, context.Guild.Id, userId);
        await _businessLogic.SavePlaylistAsync(playlist);
        await context.Channel.SendMessageAsync(Localize(nameof(MusicPlayerRessources.Message_PlaylistCreated)));
    }

    [Command("playlists")]
    public async Task ShowPlaylistsCommandAsync(ICommandContext context)
    {
        var skipPages = RequireIntArgOrDefault(context);
        var playlists = await _businessLogic.RetrieveAllPlaylistsAsync();
        var playlistsWithUserOnServer = new List<Playlist>();
        foreach (var playlist in playlists)
        {
            var owner = await context.Guild.GetUserAsync(playlist.AuthorId);
            if (owner != null)
            {
                playlistsWithUserOnServer.Add(playlist);
            }
        }

        var output = string.Empty;
        foreach (var playlist in playlistsWithUserOnServer.Skip(skipPages * 10).Take(10))
        {
            var owner = await context.Guild.GetUserAsync(playlist.AuthorId);
            output += string.Format(Localize(nameof(MusicPlayerRessources.Line_Playlist)), playlist.PlaylistId,
                playlist.Title, owner.Username);
        }

        var embedBuilder = new EmbedBuilder();
        embedBuilder.WithColor(Color.Blue);
        embedBuilder.WithCurrentTimestamp();
        embedBuilder.WithTitle(string.Format(Localize(nameof(MusicPlayerRessources.Title_Playlists)),
            context.Guild.Name, skipPages + 1));
        embedBuilder.WithDescription(output);
        await context.Channel.SendMessageAsync("", false, embedBuilder.Build());
    }

    [Command("playlist")]
    public async Task LoadPlaylistCommandAsync(ICommandContext context)
    {
        var voiceState = context.User as IVoiceState;
        if (voiceState?.VoiceChannel == null)
        {
            await context.Channel.SendMessageAsync(Localize(nameof(MusicPlayerRessources.Error_MustBeInVoice)));
            return;
        }

        var playlistId = await RequireIntArg(context);

        var playlist = await _businessLogic.RetrieveSinglePlaylistAsync(playlistId);
        if (playlist == null)
        {
            await context.Channel.SendMessageAsync(Localize(nameof(MusicPlayerRessources.Error_PlaylistNotFound)));
            return;
        }

        var count = 0;
        foreach (var track in playlist.Tracks)
        {
            if (!await PlaySongAsync(track.Title, context.Channel, context.Guild, voiceState))
                continue;
            count++;
        }

        await context.Message.AddReactionAsync(Emoji.Parse("🤝"));
        await context.Channel.SendMessageAsync(string.Format(
            Localize(nameof(MusicPlayerRessources.Message_PlaylistLoaded)), playlist.Title, count,
            playlist.Tracks.Count));
    }

    [Command("deletePlaylist")]
    public async Task DeletePlaylistCommandAsync(ICommandContext context)
    {
        var playlistId = await RequireLongArg(context);

        var playlist = await _businessLogic.RetrieveSinglePlaylistAsync(playlistId);
        if (playlist == null)
        {
            await context.Channel.SendMessageAsync(Localize(nameof(MusicPlayerRessources.Error_PlaylistNotFound)));
            return;
        }

        if (playlist.AuthorId != context.User.Id)
        {
            await context.Channel.SendMessageAsync(
                string.Format(Localize(nameof(MusicPlayerRessources.Error_PlaylistNotOwned)), playlist.Title));
            return;
        }

        await _businessLogic.DeletePlaylistAsync(playlistId);
        await context.Message.AddReactionAsync(Emoji.Parse("🤝"));
    }

    [Command("shuffle")]
    public async Task ShufflePlaylistCommandAsync(ICommandContext context)
    {
        _manager.ShufflePlaylist(context.Guild);
        await context.Message.AddReactionAsync(Emoji.Parse("🤝"));
    }
}