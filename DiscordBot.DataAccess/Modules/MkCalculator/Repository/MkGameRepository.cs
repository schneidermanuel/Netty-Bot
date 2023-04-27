using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Entities;
using DiscordBot.DataAccess.Modules.MkCalculator.Domain;
using DiscordBot.DataAccess.NHibernate;
using NHibernate.Linq;

namespace DiscordBot.DataAccess.Modules.MkCalculator.Repository;

internal class MkGameRepository : IMkGameRepository
{
    private readonly ISessionProvider _provider;

    public MkGameRepository(ISessionProvider provider)
    {
        _provider = provider;
    }

    public async Task ClearAsync(string channelId)
    {
        using (var session = _provider.OpenSession())
        {
            var entity = await session.Query<MarioKartRunnningGameEntity>()
                .Where(entity =>
                    entity.ChannelId == channelId
                    && entity.IsCompleted == false)
                .SingleOrDefaultAsync();
            if (entity != null)
            {
                entity.IsCompleted = true;
            }

            await session.SaveOrUpdateAsync(entity);

            await session.FlushAsync();
        }
    }

    public async Task<long> SaveOrUpdateGameAsync(MarioKartRunningGameData data)
    {
        using (var session = _provider.OpenSession())
        {
            var query = session.Query<MarioKartRunnningGameEntity>()
                .Where(entity => entity.ChannelId == data.ChannelId && !entity.IsCompleted);
            var entity = await query.FirstOrDefaultAsync() ?? await CreateNewGame(data);
            entity.EnemyPoints = data.EnemyPoints;
            entity.TeamPoints = data.TeamPoints;
            await session.SaveOrUpdateAsync(entity);
            await session.FlushAsync();
            return entity.GameId;
        }
    }

    public async Task SaveHistoryItemAsync(HistoryItemData historyData)
    {
        using (var session = _provider.OpenSession())
        {
            var entity = new MarioKartHistoryItemEntity
            {
                Comment = historyData.Comment,
                Id = historyData.Id,
                CreatedAt = DateTime.Now,
                EnemyPoints = historyData.EnemyPoints,
                TeamPoints = historyData.Points,
                MarioKartGameId = historyData.GameId,
                Map = historyData.Map
            };
            await session.SaveOrUpdateAsync(entity);
            await session.FlushAsync();
        }
    }

    public async Task<bool> CanRevertAsync(long gameId)
    {
        using (var session = _provider.OpenSession())
        {
            var query = session.Query<MarioKartHistoryItemEntity>()
                .Where(entity => entity.MarioKartGameId == gameId);
            return await query.AnyAsync();
        }
    }

    public async Task<HistoryItemData> RevertGameAsync(long gameId)
    {
        using (var session = _provider.OpenSession())
        {
            var query = session.Query<MarioKartHistoryItemEntity>()
                .Where(entity => entity.MarioKartGameId == gameId)
                .OrderByDescending(entity => entity.CreatedAt);
            var entity = await query.FirstAsync();
            var data = new HistoryItemData(0, gameId, entity.TeamPoints, entity.EnemyPoints, entity.Comment,
                entity.Map);
            await session.DeleteAsync(entity);
            await session.FlushAsync();
            return data;
        }
    }

    public async Task<IEnumerable<HistoryItemData>> RetrieveHistoryAsync(long gameId)
    {
        using (var session = _provider.OpenSession())
        {
            var query = session.Query<MarioKartHistoryItemEntity>()
                .Where(entity => entity.MarioKartGameId == gameId)
                .OrderBy(entity => entity.CreatedAt);
            var entities = await query.ToListAsync();
            return entities.Select(entity => new HistoryItemData(entity.Id, entity.MarioKartGameId, entity.TeamPoints,
                entity.EnemyPoints, entity.Comment, entity.Map));
        }
    }


    public async Task<IReadOnlyCollection<ulong>> RetriveChannelsToStopAsync(DateTime dueDate)
    {
        var channelIds = new List<ulong>();
        using (var session = _provider.OpenSession())
        {
            var entities = session.Query<MarioKartRunnningGameEntity>()
                .Where(entity => entity.IsCompleted == false);
            foreach (var gameEntity in entities)
            {
                try
                {
                    var lastItemCreated = await session.Query<MarioKartHistoryItemEntity>()
                        .Where(historyEntity => historyEntity.MarioKartGameId == gameEntity.GameId)
                        .Select(historyEntity => historyEntity.CreatedAt)
                        .MaxAsync();
                    if (lastItemCreated < dueDate)
                    {
                        channelIds.Add(ulong.Parse(gameEntity.ChannelId));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            return channelIds;
        }
    }

    private Task<MarioKartRunnningGameEntity> CreateNewGame(MarioKartRunningGameData data)
    {
        return Task.FromResult(new MarioKartRunnningGameEntity
        {
            ChannelId = data.ChannelId,
            GameName = data.GameName,
            IsCompleted = false
        });
    }
}