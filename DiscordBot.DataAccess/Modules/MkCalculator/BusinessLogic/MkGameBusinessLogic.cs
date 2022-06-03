using System.Threading.Tasks;
using DiscordBot.DataAccess.Contract.MkCalculator;

namespace DiscordBot.DataAccess.Modules.MkCalculator.BusinessLogic;

internal class MkGameBusinessLogic : IMkGameBusinessLogic
{
    private readonly IMkGameRepository _repository;

    public MkGameBusinessLogic(IMkGameRepository repository)
    {
        _repository = repository;
    }
    
    public async Task ClearAllAsync()
    {
        await _repository.ClearAllAsync();
    }

    public async Task ClearAsync(ulong userId)
    {
        await _repository.ClearAsync(userId.ToString());
    }
    
    public async Task SaveOrUpdate(ulong userId, MkResult gameToSave)
    {
        await _repository.SaveOrUpdate(MapToData(userId, gameToSave));
    }

    private MarioKartRunningGameData MapToData(ulong userId, MkResult gameToSave)
    {
        return new MarioKartRunningGameData(0, userId.ToString(), gameToSave.Points, gameToSave.EnemyPoints);
    }
}