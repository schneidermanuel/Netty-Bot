using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Contract.MkCalculator;

namespace DiscordBot.DataAccess.Modules.MkCalculator.Domain;

internal class MkGameDomain : IMkGameDomain
{
    private readonly IMkGameRepository _repository;

    public MkGameDomain(IMkGameRepository repository)
    {
        _repository = repository;
    }

    public async Task ClearAllAsync()
    {
        await _repository.ClearAllAsync();
    }

    public async Task ClearAsync(ulong channelId)
    {
        await _repository.ClearAsync(channelId.ToString());
    }

    public async Task<long> SaveOrUpdateAsync(ulong channelId, MkResult gameToSave)
    {
        return await _repository.SaveOrUpdateAsync(MapToData(channelId, gameToSave));
    }

    public async Task SaveHistoryItemAsync(MkHistoryItem history)
    {
        await _repository.SaveHistoryItemAsync(MapToHistoryData(history));
    }

    public async Task<bool> CanRevertAsync(long gameId)
    {
        return await _repository.CanRevertAsync(gameId);
    }

    public async Task<MkHistoryItem> RevertGameAsync(long gameId)
    {
        var data = await _repository.RevertGameAsync(gameId);
        return new MkHistoryItem
        {
            Comment = data.Comment,
            Id = data.Id,
            EnemyPoints = data.EnemyPoints,
            GameId = data.GameId,
            TeamPoints = data.Points
        };
    }

    public async Task<IEnumerable<MkHistoryItem>> RetriveHistoryAsync(long gameId)
    {
        var datas = await _repository.RetrieveHistoryAsync(gameId);
        return datas.Select(data => new MkHistoryItem
        {
            Comment = data.Comment,
            Id = data.Id,
            EnemyPoints = data.EnemyPoints,
            GameId = data.GameId,
            TeamPoints = data.Points
        });
    }

    private HistoryItemData MapToHistoryData(MkHistoryItem history)
    {
        return new HistoryItemData(history.Id, history.GameId, history.TeamPoints, history.EnemyPoints,
            history.Comment);
    }

    private MarioKartRunningGameData MapToData(ulong userId, MkResult gameToSave)
    {
        return new MarioKartRunningGameData(0, userId.ToString(), gameToSave.Points, gameToSave.EnemyPoints);
    }
}