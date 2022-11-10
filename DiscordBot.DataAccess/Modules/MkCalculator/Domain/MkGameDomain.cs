using System;
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

    public async Task ClearAsync(ulong channelId)
    {
        await _repository.ClearAsync(channelId.ToString());
    }

    public async Task<long> SaveOrUpdateAsync(ulong channelId, ulong guildId, MkResult gameToSave)
    {
        return await _repository.SaveOrUpdateAsync(MapToData(channelId, guildId, gameToSave));
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

    public async Task AutoCompleteOldGames(DateTime dueDate)
    {
        await _repository.AutoCompleteOldGames(dueDate);
    }

    private HistoryItemData MapToHistoryData(MkHistoryItem history)
    {
        return new HistoryItemData(history.Id, history.GameId, history.TeamPoints, history.EnemyPoints,
            history.Comment);
    }

    private MarioKartRunningGameData MapToData(ulong channelId, ulong guildId, MkResult gameToSave)
    {
        return new MarioKartRunningGameData(0, channelId.ToString(), guildId.ToString(), gameToSave.Points,
            gameToSave.EnemyPoints,
            $"Mario Kart match {DateTime.Now:dd.MM.yyyy hh:mm}", false);
    }
}