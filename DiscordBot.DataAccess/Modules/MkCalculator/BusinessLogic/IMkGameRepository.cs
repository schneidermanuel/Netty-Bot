using System.Threading.Tasks;

namespace DiscordBot.DataAccess.Modules.MkCalculator.BusinessLogic;

internal interface IMkGameRepository
{
    Task ClearAllAsync();
    Task ClearAsync(string channelId);
    Task SaveOrUpdate(MarioKartRunningGameData data);
}