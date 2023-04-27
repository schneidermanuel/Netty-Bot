using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.DataAccess.Contract.MkCalculator;

public interface IMkGameDomain
{
    Task ClearAsync(ulong channelId);
    Task<long> SaveOrUpdateGameAsync(ulong channelId, MkResult gameToSave);
    Task SaveHistoryItemAsync(MkHistoryItem historyItem);
    Task<bool> CanRevertAsync(long gameId);
    Task<MkHistoryItem> RevertGameAsync(long gameId);
    Task<IEnumerable<MkHistoryItem>> RetriveHistoryAsync(long gameId);
    Task<IReadOnlyCollection<ulong>> RetriveChannelsToStopAsync(DateTime dueDate);
}