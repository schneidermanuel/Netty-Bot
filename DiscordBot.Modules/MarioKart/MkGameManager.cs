using System.Collections.Generic;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Contract.MkCalculator;

namespace DiscordBot.Modules.MarioKart;

internal class MkManager
{
    private readonly IMkGameBusinessLogic _businessLogic;
    private readonly Dictionary<ulong, MkResult> _runningGames = new();

    public MkManager(IMkGameBusinessLogic businessLogic)
    {
        _businessLogic = businessLogic;
    }
    
    public async Task RegisterResultAsync(MkResult result, ulong channel, string comment)
    {
        if (!_runningGames.ContainsKey(channel))
        {
            _runningGames.Add(channel, new MkResult());
        }

        var game = _runningGames[channel];
        game.Points += result.Points;
        game.EnemyPoints += result.EnemyPoints;
        var gameId = await _businessLogic.SaveOrUpdateAsync(channel, game);
        _runningGames[channel].GameId = gameId;
        var history = new MkHistoryItem
        {
            Comment = comment,
            Id = 0,
            EnemyPoints = result.EnemyPoints,
            TeamPoints = result.Points,
            GameId = gameId
        };
        await _businessLogic.SaveHistoryItemAsync(history);
    }
    
    public MkResult GetFinalResult(ulong channelId)
    {
        if (!_runningGames.ContainsKey(channelId))
        {
            return null;
        }

        var game = _runningGames[channelId];
        return game;
    }

    public void EndGame(ulong channelId)
    {
        if (_runningGames.ContainsKey(channelId))
        {
            _runningGames.Remove(channelId);
            _businessLogic.ClearAsync(channelId);
        }
    }

    public async Task<bool> CanRevertAsync(ulong channelId)
    {
        if (!_runningGames.ContainsKey(channelId))
        {
            return false;
        }

        var gameId = _runningGames[channelId].GameId;
        return await _businessLogic.CanRevertAsync(gameId);
    }

    public async Task RevertGameAsync(ulong channelId)
    {
        var gameId = _runningGames[channelId].GameId;
        var historyItem = await _businessLogic.RevertGameAsync(gameId);

        var result = _runningGames[channelId];
        result.Points -= historyItem.TeamPoints;
        result.EnemyPoints -= historyItem.EnemyPoints;

    }
}