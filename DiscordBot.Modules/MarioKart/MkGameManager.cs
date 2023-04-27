using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Contract.MkCalculator;
using Microsoft.VisualBasic;

namespace DiscordBot.Modules.MarioKart;

internal class MkGameManager
{
    private readonly IMkGameDomain _domain;
    private readonly Dictionary<ulong, MkGame> _runningGames = new();
    private readonly Dictionary<ulong, MkResult> _resultsInProcess = new();

    public MkGameManager(IMkGameDomain domain)
    {
        _domain = domain;
    }

    public bool HasChannelRunningGame(ulong channelId)
    {
        return _runningGames.ContainsKey(channelId);
    }

    public async Task StartGameAsync(ulong channelId, MkGame game)
    {
        var preparedRace = _resultsInProcess[channelId];
        var gameId = await _domain.SaveOrUpdateGameAsync(channelId, preparedRace);
        _runningGames.Add(channelId, game);
        game.GameId = gameId;
        await RegisterResultAsync(preparedRace, channelId);
    }

    public async Task RegisterResultAsync(MkResult result, ulong channelId)
    {
        if (!_runningGames.ContainsKey(channelId))
        {
            _resultsInProcess.Add(channelId, result);
            return;
        }

        var game = _runningGames[channelId];
        result.Track = game.Races.Count + 1;

        var history = new MkHistoryItem
        {
            Comment = result.Comment,
            Id = 0,
            EnemyPoints = result.EnemyPoints,
            TeamPoints = result.Points,
            GameId = game.GameId,
            Map = result.Map
        };
        await _domain.SaveHistoryItemAsync(history);
    }

    public MkGame RetrieveGame(ulong channelId)
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
        var game = _runningGames[channelId];
        game.Races.Remove(game.Races.OrderBy(r => r.Track).Last());
        await _domain.RevertGameAsync(game.GameId);
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

    public async Task AutoCompleteGamesAsync(DateTime dueDate)
    {
        var gamesToStop = await _domain.RetriveChannelsToStopAsync(dueDate);
        var stopGameTasks = gamesToStop.Select(EndGameAsync);
        await Task.WhenAll(stopGameTasks);
    }
}