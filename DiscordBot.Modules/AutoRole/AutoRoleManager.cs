using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract.AutoRole;

namespace DiscordBot.Modules.AutoRole;

internal class AutoRoleManager
{
    private readonly IAutoRoleDomain _domain;
    private IList<AutoRoleSetup> _setups;

    public AutoRoleManager(IAutoRoleDomain domain)
    {
        _domain = domain;
    }

    public async Task RefreshSetupsAsync()
    {
        _setups = (await _domain.RetrieveAllSetupsAsync()).ToList();
    }

    public async Task RefreshGuildAsync(ulong guildId)
    {
        var newSetupsTask = _domain.RetrieveAllSetupsForGuildAsync(guildId);

        var setupsForGuild = _setups.Where(setup => setup.GuildId == guildId).ToArray();
        foreach (var autoRoleSetup in setupsForGuild)
        {
            _setups.Remove(autoRoleSetup);
        }

        var newSetups = await newSetupsTask;
        foreach (var autoRoleSetup in newSetups)
        {
            _setups.Add(autoRoleSetup);
        }
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