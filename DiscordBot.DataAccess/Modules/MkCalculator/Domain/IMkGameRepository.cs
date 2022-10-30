using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.DataAccess.Modules.MkCalculator.Domain;

internal interface IMkGameRepository
{
    Task ClearAllAsync();
    Task ClearAsync(string channelId);
    Task<long> SaveOrUpdateAsync(MarioKartRunningGameData data);
    Task SaveHistoryItemAsync(HistoryItemData historyData);
    Task<bool> CanRevertAsync(long gameId);
    Task<HistoryItemData> RevertGameAsync(long gameId);
    Task<IEnumerable<HistoryItemData>> RetrieveHistoryAsync(long gameId);
}