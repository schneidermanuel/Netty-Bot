using System.Collections.Generic;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Contract.MkCalculator;

namespace DiscordBot.Modules.MarioKart;

internal class MkManager
{
    private readonly IMkGameDomain _domain;
    private readonly Dictionary<ulong, MkResult> _runningGames = new();

    public MkManager(IMkGameDomain domain)
    {
        _domain = domain;
    }

    public async Task RegisterResultAsync(MkResult result, ulong channel, ulong guild, string comment)
    {
        if (!_runningGames.ContainsKey(channel))
        {
            _runningGames.Add(channel, new MkResult());
        }

        var game = _runningGames[channel];
        game.Points += result.Points;
        game.EnemyPoints += result.EnemyPoints;
        var gameId = await _domain.SaveOrUpdateAsync(channel, guild, game);
        _runningGames[channel].GameId = gameId;
        var history = new MkHistoryItem
        {
            Comment = comment,
            Id = 0,
            EnemyPoints = result.EnemyPoints,
            TeamPoints = result.Points,
            GameId = gameId
        };
        await _domain.SaveHistoryItemAsync(history);
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

    public async Task<bool> CanRevertAsync(ulong channelId)
    {
        if (!_runningGames.ContainsKey(channelId))
        {
            return false;
        }

        var gameId = _runningGames[channelId].GameId;
        return await _domain.CanRevertAsync(gameId);
    }

    public async Task RevertGameAsync(ulong channelId)
    {
        var gameId = _runningGames[channelId].GameId;
        var historyItem = await _domain.RevertGameAsync(gameId);

        var result = _runningGames[channelId];
        result.Points -= historyItem.TeamPoints;
        result.EnemyPoints -= historyItem.EnemyPoints;
    }

    public async Task<IEnumerable<MkHistoryItem>> RetriveHistoryAsync(long gameId)
    {
        return await _domain.RetriveHistoryAsync(gameId);
    }

    public async Task EndGameAsync(ulong channelId)
    {
        if (_runningGames.ContainsKey(channelId))
        {
            _runningGames.Remove(channelId);
            await _domain.ClearAsync(channelId);
        }
    }
}