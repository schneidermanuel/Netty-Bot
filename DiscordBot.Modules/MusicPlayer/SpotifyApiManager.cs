using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DiscordBot.Framework.Contract;
using DiscordBot.Framework.Contract.Boot;
using Newtonsoft.Json.Linq;
using SpotifyAPI.Web;

namespace DiscordBot.Modules.MusicPlayer;

internal class SpotifyApiManager : IBootStep
{
    private const string SpotifyTokenUrl = "https://accounts.spotify.com/api/token";
    private SpotifyClient _spotify;

    public async Task BootAsync()
    {
        var bearerToken = await AuthorizeSpotifyAsync();
        _spotify = new SpotifyClient(bearerToken);
    }

    private async Task<string> AuthorizeSpotifyAsync()
    {
        var http = new HttpClient();
        var content = new FormUrlEncodedContent(new[]
            { new KeyValuePair<string, string>("grant_type", "client_credentials") });
        var authcode =
            Base64Encode($"{BotClientConstants.SpotifyClientId}:{BotClientConstants.SpotifyClientSecret}");
        http.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Basic {authcode}");
        http.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");

        var result = await http.PostAsync(SpotifyTokenUrl, content);
        var json = await result.Content.ReadAsStringAsync();
        var token = JObject.Parse(json)["access_token"]?.ToString() ?? string.Empty;
        return token;
    }

    public async Task<IReadOnlyCollection<string>> GetPlaylistAsync(string playlistId)
    {
        var result = new List<string>();
        await foreach (var track in _spotify.Paginate(await _spotify.Playlists.GetItems(playlistId)))
        {
            try
            {
                var fullTrack = (FullTrack)track.Track;
                result.Add(fullTrack.Name + " - " + fullTrack.Artists.First().Name);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        return result;
    }


    private static string Base64Encode(string plainText)
    {
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(plainTextBytes);
    }

    public BootOrder StepPosition => BootOrder.Async;

    public async Task<SpotifyPlaylistInfo> GetPlaylistInformationAsync(string playlistId)
    {
        var playlist = await _spotify.Playlists.Get(playlistId);
        var pages = await _spotify.Paginate(await _spotify.Playlists.GetItems(playlistId)).CountAsync();
        return new SpotifyPlaylistInfo
        {
            Name = playlist.Name,
            TrackCount = pages
        };
    }
}