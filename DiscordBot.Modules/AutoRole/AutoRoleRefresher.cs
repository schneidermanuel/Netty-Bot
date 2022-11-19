using System.Threading.Tasks;
using DiscordBot.Framework.Contract.Modules.AutoRole;

namespace DiscordBot.Modules.AutoRole;

internal class AutoRoleRefresher : IAutoRoleRefresher
{
    private readonly AutoRoleManager _manager;

    public AutoRoleRefresher(AutoRoleManager manager)
    {
        _manager = manager;
    }

    public async Task RefreshAsync(ulong guildId)
    {
        await _manager.RefreshGuildAsync(guildId);
    }
}