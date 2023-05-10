using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Contract.MkCalculator;

namespace DiscordBot.DataAccess.Modules.MkCalculator.Domain;

internal interface IMkGameRepository
{
    Task ClearAsync(string channelId);
    Task<bool> CanRevertAsync(long gameId);
    Task RevertGameAsync(long gameId);
    Task<IReadOnlyCollection<ulong>> RetrieveChannelsToStopAsync(DateTime dueDate);
    Task<long> SaveGameAsync(MkGame gameToSave, string gameName, string guildId, string channelId);
    Task<long> SaveRaceAsync(MkResult result, DateTime createdAt, long gameId);
    Task<MkGame> RetrieveGameAsync(long gameId);
    Task UpdateTotalsAsync(MkGame game);
}