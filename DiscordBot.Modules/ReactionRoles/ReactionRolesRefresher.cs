using System;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Contract.ReactionRoles;
using DiscordBot.Framework.Contract.Modules.ReactionRoles;

namespace DiscordBot.Modules.ReactionRoles;

internal class ReactionRolesRefresher : IReactionRoleRefresher
{
    private readonly ReactionRoleManager _manager;
    private readonly IReactionRoleDomain _domain;

    public ReactionRolesRefresher(ReactionRoleManager manager, IReactionRoleDomain domain)
    {
        _manager = manager;
        _domain = domain;
    }

    public async Task RefreshAsync(ulong guildId)
    {
        var roles = await _domain.RetrieveReactionRolesForGuildAsync(guildId);
        await _manager.RefreshGuildAsync(guildId, roles);
    }
}