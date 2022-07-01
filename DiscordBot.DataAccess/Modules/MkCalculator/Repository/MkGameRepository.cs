using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Entities;
using DiscordBot.DataAccess.Modules.MkCalculator.BusinessLogic;
using DiscordBot.DataAccess.NHibernate;
using NHibernate;
using NHibernate.Linq;

namespace DiscordBot.DataAccess.Modules.MkCalculator.Repository;

internal class MkGameRepository : IMkGameRepository
{
    private readonly ISessionProvider _provider;

    public MkGameRepository(ISessionProvider provider)
    {
        _provider = provider;
    }

    public async Task ClearAllAsync()
    {
        using (var session = _provider.OpenSession())
        {
            var entities = session.Query<MarioKartRunnningGameEntity>();
            foreach (var entity in entities)
            {
                await DeleteHistoryAsync(entity.GameId, session);
                await session.DeleteAsync(entity);
            }

            await session.FlushAsync();
        }
    }

    public async Task ClearAsync(string channelId)
    {
        using (var session = _provider.OpenSession())
        {
            var entities = session.Query<MarioKartRunnningGameEntity>().Where(entity => entity.ChannelId == channelId);
            foreach (var entity in entities)
            {
                await DeleteHistoryAsync(entity.GameId, session);
                await session.DeleteAsync(entity);
            }

            await session.FlushAsync();
        }
    }

    private async Task DeleteHistoryAsync(long entityGameId, ISession session)
    {
        var query = session.Query<MarioKartHistoryItemEntity>()
            .Where(entity => entity.MarioKartGameId == entityGameId);
        foreach (var entity in query)
        {
            await session.DeleteAsync(entity);
        }
    }

    public async Task<long> SaveOrUpdateAsync(MarioKartRunningGameData data)
    {
        using (var session = _provider.OpenSession())
        {
            var query = session.Query<MarioKartRunnningGameEntity>().Where(entity => entity.ChannelId == data.ChannelId);
            var entity = await query.FirstOrDefaultAsync() ?? await CreateNewGameAsync(data.ChannelId);
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
                MarioKartGameId = historyData.GameId
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
            var data = new HistoryItemData(0, gameId, entity.TeamPoints, entity.EnemyPoints, entity.Comment);
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
            return entities.Select(entity => new HistoryItemData(entity.Id, entity.MarioKartGameId, entity.TeamPoints, entity.EnemyPoints, entity.Comment));
        }
    }

    private async Task<MarioKartRunnningGameEntity> CreateNewGameAsync(string channelId)
    {
        using (var session = _provider.OpenSession())
        {
            var existingGamesQuery =
                session.Query<MarioKartRunnningGameEntity>().Where(entity => entity.ChannelId == channelId);
            foreach (var existingGame in existingGamesQuery)
            {
                await session.DeleteAsync(existingGame);
            }

            await session.FlushAsync();
        }

        return new MarioKartRunnningGameEntity
        {
            ChannelId = channelId
        };
    }
}