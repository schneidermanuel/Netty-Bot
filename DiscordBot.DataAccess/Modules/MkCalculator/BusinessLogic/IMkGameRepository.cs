using System.Threading.Tasks;

namespace DiscordBot.DataAccess.Modules.MkCalculator.BusinessLogic;

internal interface IMkGameRepository
{
    Task ClearAllAsync();
    Task ClearAsync(string channelId);
    Task<long> SaveOrUpdateAsync(MarioKartRunningGameData data);
    Task SaveHistoryItemAsync(HistoryItemData historyData);
    Task<bool> CanRevertAsync(long gameId);
    Task<HistoryItemData> RevertGameAsync(long gameId);
}