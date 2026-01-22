using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Contract.MkCalculator;
using DiscordBot.DataAccess.Entities;
using DiscordBot.DataAccess.Hibernate;
using DiscordBot.DataAccess.Modules.MkCalculator.Domain;
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
            var entities = await session.Query<MarioKartRunningGameEntity>()
                .Where(entity =>
                    entity.ChannelId == channelId
                    && entity.IsCompleted == false).ToListAsync();
            foreach (var entity in entities)
            {
                entity.IsCompleted = true;
                await session.SaveOrUpdateAsync(entity);
            }

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

    public async Task RevertGameAsync(long gameId)
    {
        using (var session = _provider.OpenSession())
        {
            var query = session.Query<MarioKartHistoryItemEntity>()
                .Where(entity => entity.MarioKartGameId == gameId)
                .OrderByDescending(entity => entity.CreatedAt);
            var entity = await query.FirstAsync();
            await session.DeleteAsync(entity);
            await session.FlushAsync();
        }
    }

    public async Task<IReadOnlyCollection<ulong>> RetrieveChannelsToStopAsync(DateTime dueDate)
    {
        var channelIds = new List<ulong>();
        using (var session = _provider.OpenSession())
        {
            var entities = session.Query<MarioKartRunningGameEntity>()
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

    public async Task<long> SaveGameAsync(MkGame gameToSave, string gameName, string guildId, string channelId)
    {
        using (var session = _provider.OpenSession())
        {
            var game = new MarioKartRunningGameEntity
            {
                ChannelId = channelId,
                IsCompleted = false,
                GameId = 0,
                GameName = gameName,
                EnemyImage = gameToSave.Enemy.Image,
                EnemyName = gameToSave.Enemy.Name,
                EnemyPoints = 0,
                GuildId = guildId,
                TeamImage = gameToSave.Team.Image,
                TeamName = gameToSave.Team.Name,
                TeamPoints = 0
            };
            await session.SaveOrUpdateAsync(game);
            await session.FlushAsync();
            return game.GameId;
        }
    }

    public async Task<long> SaveRaceAsync(MkResult result, DateTime createdAt, long gameId)
    {
        using (var session = _provider.OpenSession())
        {
            var entity = new MarioKartHistoryItemEntity
            {
                CreatedAt = createdAt,
                EnemyPoints = result.EnemyPoints,
                TeamPoints = result.Points,
                MarioKartGameId = gameId,
                Comment = result.Comment,
                Map = result.Map
            };
            await session.SaveOrUpdateAsync(entity);
            await session.FlushAsync();
            return entity.Id;
        }
    }

    public async Task<MkGame> RetrieveGameAsync(long gameId)
    {
        using (var session = _provider.OpenSession())
        {
            var gameEntity = await session.LoadAsync<MarioKartRunningGameEntity>(gameId);
            var raceEntities = (await session.Query<MarioKartHistoryItemEntity>()
                    .Where(race => race.MarioKartGameId == gameId)
                    .ToListAsync())
                .OrderBy(race => race.CreatedAt)
                .ToArray();
            var game = new MkGame
            {
                Enemy = new MkTeam
                {
                    Image = gameEntity.EnemyImage,
                    Name = gameEntity.EnemyName
                },
                GameId = gameEntity.GameId,
                Team = new MkTeam
                {
                    Image = gameEntity.TeamImage,
                    Name = gameEntity.TeamName
                },
                Races = raceEntities.Select(entity => new MkResult
                    {
                        Comment = entity.Comment,
                        Map = entity.Map,
                        EnemyPoints = entity.EnemyPoints,
                        Points = entity.TeamPoints,
                        Track = Array.IndexOf(raceEntities, entity)
                    })
                    .ToArray()
            };
            return game;
        }
    }

    public async Task UpdateTotalsAsync(MkGame game)
    {
        using (var session = _provider.OpenSession())
        {
            var entity = await session.LoadAsync<MarioKartRunningGameEntity>(game.GameId);
            entity.TeamPoints = game.Totals.Points;
            entity.EnemyPoints = game.Totals.EnemyPoints;
            await session.SaveOrUpdateAsync(entity);
            await session.FlushAsync();
        }
    }
}