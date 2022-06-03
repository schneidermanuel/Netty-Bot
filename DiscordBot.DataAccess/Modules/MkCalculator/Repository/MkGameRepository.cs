using System.Linq;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Entities;
using DiscordBot.DataAccess.Modules.MkCalculator.BusinessLogic;
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

    public async Task ClearAllAsync()
    {
        using (var session = _provider.OpenSession())
        {
            var entities = session.Query<MarioKartRunnningGameEntity>();
            foreach (var entity in entities)
            {
                await session.DeleteAsync(entity);
            }

            await session.FlushAsync();
        }
    }

    public async Task ClearAsync(string userId)
    {
        using (var session = _provider.OpenSession())
        {
            var entities = session.Query<MarioKartRunnningGameEntity>().Where(entity => entity.UserId == userId);
            foreach (var entity in entities)
            {
                await session.DeleteAsync(entity);
            }

            await session.FlushAsync();
        }
    }

    public async Task SaveOrUpdate(MarioKartRunningGameData data)
    {
        using (var session = _provider.OpenSession())
        {
            var query = session.Query<MarioKartRunnningGameEntity>().Where(entity => entity.UserId == data.UserId);
            var entity = await query.FirstOrDefaultAsync() ?? await CreateNewGameAsync(data.UserId);
            entity.EnemyPoints = data.EnemyPoints;
            entity.TeamPoints = data.TeamPoints;
            await session.SaveOrUpdateAsync(entity);
            await session.FlushAsync();
        }
    }

    private async Task<MarioKartRunnningGameEntity> CreateNewGameAsync(string userId)
    {
        using (var session = _provider.OpenSession())
        {
            var existingGamesQuery =
                session.Query<MarioKartRunnningGameEntity>().Where(entity => entity.UserId == userId);
            foreach (var existingGame in existingGamesQuery)
            {
                await session.DeleteAsync(existingGame);
            }

            await session.FlushAsync();
        }

        return new MarioKartRunnningGameEntity
        {
            UserId = userId
        };
    }
}