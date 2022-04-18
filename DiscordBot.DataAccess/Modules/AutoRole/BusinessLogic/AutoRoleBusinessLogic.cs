using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Contract.AutoRole;
using DiscordBot.DataAccess.Modules.AutoRole.Repository;

namespace DiscordBot.DataAccess.Modules.AutoRole.BusinessLogic;

internal class AutoRoleBusinessLogic : IAutoRoleBusinessLogic
{
    private readonly IAutoRoleRepository _repository;

    public AutoRoleBusinessLogic(IAutoRoleRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> CanCreateAutoRoleAsync(ulong guildId, ulong roleId)
    {
        return await _repository.CanCreateAutoRoleAsync(guildId, roleId);
    }

    public async Task SaveSetupAsync(AutoRoleSetup setup)
    {
        var data = new AutoRoleSetupData(setup.AutoRoleSetupId, setup.GuildId, setup.RoleId);
        await _repository.SaveSetupAsync(data);
    }

    public async Task<IEnumerable<AutoRoleSetup>> RetrieveAllSetupsForGuildAsync(ulong guildId)
    {
        var datas = await _repository.RetrieveAllSetupsForGuildAsync(guildId);
        return datas.Select(MapToDto);
    }

    public async Task DeleteSetupAsync(long autoRoleSetupId)
    {
        await _repository.DeleteSetupAsync(autoRoleSetupId);
    }

    public async Task<IEnumerable<AutoRoleSetup>> RetrieveAllSetupsAsync()
    {
        var datas = await _repository.RetrieveAllSetupsAsync();
        return datas.Select(MapToDto);
    }

    private AutoRoleSetup MapToDto(AutoRoleSetupData data)
    {
        return new AutoRoleSetup
        {
            GuildId = ulong.Parse(data.GuildId),
            RoleId = ulong.Parse(data.RoleId),
            AutoRoleSetupId = data.Id
        };
    }
}