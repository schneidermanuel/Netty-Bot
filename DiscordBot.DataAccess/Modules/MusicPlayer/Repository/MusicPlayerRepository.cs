using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Entities;
using DiscordBot.DataAccess.Modules.MusicPlayer.Domain;
using DiscordBot.DataAccess.NHibernate;
using NHibernate.Linq;

namespace DiscordBot.DataAccess.Modules.MusicPlayer.Repository;

internal class MusicPlayerRepository : IMusicPlayerRepository
{
    private readonly ISessionProvider _sessionProvider;

    private readonly Dictionary<string, int> _countOverrides = new()
    {
        { "382248892101558274", 100 }
    };

    public MusicPlayerRepository(ISessionProvider sessionProvider)
    {
        _sessionProvider = sessionProvider;
    }

    public async Task<bool> CanUserCreatePlaylistAsync(string userId)
    {
        using (var session = _sessionProvider.OpenSession())
        {
            var query = session.Query<PlaylistEntity>()
                .Where(playlist => playlist.UserId == userId);
            var count = await query.CountAsync();
            if (!_countOverrides.ContainsKey(userId)) return count == 0;
            var availableCount = _countOverrides[userId];
            return availableCount > count;
        }
    }

    public async Task<long> SavePlaylistAsync(PlaylistData playlistData)
    {
        var entity = new PlaylistEntity
        {
            Title = playlistData.Title,
            PlaylistId = playlistData.PlaylistId,
            UserId = playlistData.UserId
        };
        using (var session = _sessionProvider.OpenSession())
        {
            await session.SaveOrUpdateAsync(entity);
            await session.FlushAsync();
            return entity.PlaylistId;
        }
    }

    public async Task SaveTrackAsync(PlaylistItemData track)
    {
        var entity = new PlaylistItemEntity
        {
            Url = track.Url,
            PlaylistId = track.PlaylistId,
            PlaylistItemId = track.PlaylistItemId
        };
        using (var session = _sessionProvider.OpenSession())
        {
            await session.SaveOrUpdateAsync(entity);
            await session.FlushAsync();
        }
    }

    public async Task<IEnumerable<PlaylistData>> RetrieveAllPLaylistsAsync()
    {
        using (var session = _sessionProvider.OpenSession())
        {
            var query = session.Query<PlaylistEntity>();
            var entities = await query.ToListAsync();
            return entities.Select(MapPlaylistEntityToData);
        }
    }

    public async Task<IEnumerable<PlaylistItemData>> RetrieveTracksForPlaylistAsync(PlaylistData playlistData)
    {
        using (var session = _sessionProvider.OpenSession())
        {
            var query = session.Query<PlaylistItemEntity>()
                .Where(entity => entity.PlaylistId == playlistData.PlaylistId);
            var entities = await query.ToListAsync();
            return entities.Select(MapTrackToData);
        }
    }

    public async Task<PlaylistData> RetrievePlaylistDataAsync(long playlistId)
    {
        using (var session = _sessionProvider.OpenSession())
        {
            var query = session.Query<PlaylistEntity>()
                .Where(playlist => playlist.PlaylistId == playlistId);
            var entity = await query.SingleOrDefaultAsync();
            return entity == null ? null : MapPlaylistEntityToData(entity);
        }
    }

    public async Task DeletePlaylistAsync(long playlistId)
    {
        using (var session = _sessionProvider.OpenSession())
        {
            var playlist = await session.GetAsync<PlaylistEntity>(playlistId);
            var query = session.Query<PlaylistItemEntity>().Where(item => item.PlaylistId == playlistId);
            foreach (var song in query)
            {
                await session.DeleteAsync(song);
            }

            await session.DeleteAsync(playlist);
            await session.FlushAsync();
        }
    }

    private PlaylistItemData MapTrackToData(PlaylistItemEntity entity)
    {
        return new PlaylistItemData(entity.PlaylistItemId, entity.Url, entity.PlaylistId);
    }

    private PlaylistData MapPlaylistEntityToData(PlaylistEntity entity)
    {
        return new PlaylistData(entity.PlaylistId, entity.UserId, entity.Title);
    }
}