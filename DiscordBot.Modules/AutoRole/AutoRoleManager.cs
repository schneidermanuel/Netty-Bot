using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract.AutoRole;

namespace DiscordBot.Modules.AutoRole;

internal class AutoRoleManager
{
    private readonly IAutoRoleDomain _domain;
    private IEnumerable<AutoRoleSetup> _setups;

    public AutoRoleManager(IAutoRoleDomain domain)
    {
        _domain = domain;
    }

    public async Task RefreshSetupsAsync()
    {
        _setups = await _domain.RetrieveAllSetupsAsync();
    }

    public async Task UserJoinedGuild(SocketGuildUser user)
    {
        var roles = _setups.Where(setup => setup.GuildId == user.Guild.Id).ToArray();
        foreach (var roleSetup in roles)
        {
            var role = user.Guild.GetRole(roleSetup.RoleId);
            if (role != null)
            {
                await user.AddRoleAsync(role);
            }
        }
    }
}