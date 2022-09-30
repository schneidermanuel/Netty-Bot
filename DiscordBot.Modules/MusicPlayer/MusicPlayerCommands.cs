using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract;
using DiscordBot.DataAccess.Contract.MusicPlayer;
using DiscordBot.Framework.Contract.Modularity;
using Victoria;

namespace DiscordBot.Modules.MusicPlayer;

internal class MusicPlayerCommands : CommandModuleBase, ICommandModule
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


    [Command("play")]
    [Description("Play a song")]
    [Parameter(Name = "song", Description = "The song to play", IsOptional = false,
        ParameterType = ApplicationCommandOptionType.String)]
    public async Task PlayCommand(SocketSlashCommand context)
    {
        await context.DeferAsync();
        var songname = await RequireString(context);
        try
        {
            var guild = await RequireGuild(context);
            var voiceState = context.User as IVoiceState;
            if (!await PlaySongAsync(songname, context.Channel, guild, voiceState)) return;
            await context.ModifyOriginalResponseAsync(options => options.Content = "🤝");
        }
        catch (Exception e)
        {
            await context.ModifyOriginalResponseAsync(options =>
                options.Content = string.Format(Localize(nameof(MusicPlayerRessources.Error_SongNotFound)), songname));
            Console.WriteLine(e);
        }
    }

    [Command("loadSpotify")]
    [Description("Plays a playlist from spotify")]
    [Parameter(Name = "URL", Description = "The URL of the playlist. Needs to be public", IsOptional = false,
        ParameterType = ApplicationCommandOptionType.String)]
    public async Task LoadSpotifyAsync(SocketSlashCommand context)
    {
        var voiceState = context.User as IVoiceState;
        if (voiceState?.VoiceChannel == null)
        {
            await context.RespondAsync(Localize(nameof(MusicPlayerRessources.Error_MustBeInVoice)));
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
            var guild = await RequireGuild(context);

            await PlaySongAsync(tracks.First(), context.Channel, guild, voiceState);

            var results = await Task.WhenAll(tracks.Skip(1)
                .Select(track => PlaySongAsync(track, context.Channel, guild, voiceState)));

            var count = results.Count(result => result);

            await context.RespondAsync(string.Format(
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
    [Description("Skips the current song")]
    public async Task SkipCommand(SocketSlashCommand context)
    {
        var guild = await RequireGuild(context);

        if (!_lavaNode.HasPlayer(guild))
        {
            await context.RespondAsync(Localize(nameof(MusicPlayerRessources.Error_NoMusic)));
            return;
        }

        await _manager.SkipTrack(guild);
        await context.RespondAsync("🤝");
    }

    [Command("queue")]
    [Description("Shows the current queue")]
    [Parameter(Name = "page", Description = "The page to show", IsOptional = true,
        ParameterType = ApplicationCommandOptionType.Integer)]
    public async Task Queue(SocketSlashCommand context)
    {
        var guild = await RequireGuild(context);

        var skipPages = RequireIntArgOrDefault(context, 1, 1) - 1;
        var songCount = _manager.GetSongCount(guild);
        if (songCount == 0)
        {
            await context.RespondAsync(Localize(nameof(MusicPlayerRessources.Error_NoMusic)));
            return;
        }

        var pageCount = (songCount / 10) + 1;
        if (skipPages > pageCount)
        {
            await context.RespondAsync(Localize(nameof(MusicPlayerRessources.Error_InvlaidPage)));
            return;
        }

        var songs = _manager.RetrieveSongs(guild, skipPages);
        var embedBuilder = new EmbedBuilder();
        embedBuilder.WithColor(Color.Blue);
        embedBuilder.WithDescription(songs);
        embedBuilder.WithCurrentTimestamp();
        embedBuilder.WithTitle(string.Format(Localize(nameof(MusicPlayerRessources.Title_QueueTitle)), songCount,
            skipPages + 1, pageCount));
        await context.RespondAsync(string.Empty, new[] { embedBuilder.Build() });
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
    [Description("Pauses the current song")]
    public async Task PauseCommand(SocketSlashCommand context)
    {
        var guild = await RequireGuild(context);

        if (!_lavaNode.HasPlayer(guild))
        {
            await context.RespondAsync(Localize(nameof(MusicPlayerRessources.Error_NoMusic)));
            return;
        }

        await _manager.PauseAsync(guild);
        await context.RespondAsync("🤝");
    }

    [Command("resume")]
    [Description("Resumes the current song")]
    public async Task ResumeCommand(SocketSlashCommand context)
    {
        var guild = await RequireGuild(context);

        if (!_lavaNode.HasPlayer(guild))
        {
            await context.RespondAsync(Localize(nameof(MusicPlayerRessources.Error_NoMusic)));
            return;
        }

        await _manager.ResumeAsync(guild);
        await context.RespondAsync("🤝");
    }

    [Command("createPlaylist")]
    [Description("Creates a playlist from the current queue")]
    [Parameter(Name = "name", Description = "The name of the playlist", IsOptional = false,
        ParameterType = ApplicationCommandOptionType.String)]
    public async Task SaveQueueToDatabaseAsync(SocketSlashCommand context)
    {
        var userId = context.User.Id;
        var canUserCreatePlaylist = await _businessLogic.CanUserCreatePlaylistAsync(userId);
        if (!canUserCreatePlaylist)
        {
            await context.RespondAsync(Localize(nameof(MusicPlayerRessources.Error_TooManyPlaylists)));
            return;
        }

        var guild = await RequireGuild(context);

        var title = await RequireString(context);
        var playlist = _manager.CreatePlaylist(title, guild.Id, userId);
        await _businessLogic.SavePlaylistAsync(playlist);
        await context.RespondAsync(Localize(nameof(MusicPlayerRessources.Message_PlaylistCreated)));
    }

    [Command("playlists")]
    [Description("Shows all playlists")]
    [Parameter(Name = "page", Description = "The page to show", IsOptional = true,
        ParameterType = ApplicationCommandOptionType.Integer)]
    public async Task ShowPlaylistsCommandAsync(SocketSlashCommand context)
    {
        var guild = await RequireGuild(context);

        var skipPages = RequireIntArgOrDefault(context, 1, 1) - 1;
        var playlists = await _businessLogic.RetrieveAllPlaylistsAsync();
        var playlistsWithUserOnServer = new List<Playlist>();
        foreach (var playlist in playlists)
        {
            var owner = await guild.GetUserAsync(playlist.AuthorId);
            if (owner != null)
            {
                playlistsWithUserOnServer.Add(playlist);
            }
        }

        var output = string.Empty;
        foreach (var playlist in playlistsWithUserOnServer.Skip(skipPages * 10).Take(10))
        {
            var owner = await guild.GetUserAsync(playlist.AuthorId);
            output += string.Format(Localize(nameof(MusicPlayerRessources.Line_Playlist)), playlist.PlaylistId,
                playlist.Title, owner.Username);
        }

        var embedBuilder = new EmbedBuilder();
        embedBuilder.WithColor(Color.Blue);
        embedBuilder.WithCurrentTimestamp();
        embedBuilder.WithTitle(string.Format(Localize(nameof(MusicPlayerRessources.Title_Playlists)), guild.Name,
            skipPages + 1));
        embedBuilder.WithDescription(output);
        await context.RespondAsync("", new[] { embedBuilder.Build() });
    }

    [Command("playlist")]
    [Description("Plays a playlist")]
    [Parameter(Name = "playlistId", Description = "The id of the playlist", IsOptional = false,
        ParameterType = ApplicationCommandOptionType.Integer)]
    public async Task LoadPlaylistCommandAsync(SocketSlashCommand context)
    {
        var guild = await RequireGuild(context);

        var voiceState = context.User as IVoiceState;
        if (voiceState?.VoiceChannel == null)
        {
            await context.RespondAsync(Localize(nameof(MusicPlayerRessources.Error_MustBeInVoice)));
            return;
        }

        var playlistId = await RequireIntArg(context);

        var playlist = await _businessLogic.RetrieveSinglePlaylistAsync(playlistId);
        if (playlist == null)
        {
            await context.RespondAsync(Localize(nameof(MusicPlayerRessources.Error_PlaylistNotFound)));
            return;
        }

        var count = 0;
        foreach (var track in playlist.Tracks)
        {
            if (!await PlaySongAsync(track.Title, context.Channel, guild, voiceState))
                continue;
            count++;
        }

        await context.RespondAsync(string.Format(
            Localize(nameof(MusicPlayerRessources.Message_PlaylistLoaded)), playlist.Title, count,
            playlist.Tracks.Count));
    }

    [Command("deletePlaylist")]
    [Description("Deletes a playlist")]
    [Parameter(Name = "playlistId", Description = "The id of the playlist", IsOptional = false,
        ParameterType = ApplicationCommandOptionType.Integer)]
    public async Task DeletePlaylistCommandAsync(SocketSlashCommand context)
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
        await context.RespondAsync("🤝");
    }

    [Command("shuffle")]
    [Description("Shuffles the current queue")]
    public async Task ShufflePlaylistCommandAsync(SocketSlashCommand context)
    {
        var guild = await RequireGuild(context);

        _manager.ShufflePlaylist(guild);
        await context.RespondAsync("🤝");
    }
}