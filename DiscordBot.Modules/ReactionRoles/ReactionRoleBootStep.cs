using System.Linq;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Contract.ReactionRoles;
using DiscordBot.Framework.Contract.Boot;

namespace DiscordBot.Modules.ReactionRoles;

public class ReactionRoleBootStep : IBootStep
{
    private readonly IReactionRoleDomain _domain;
    private readonly ReactionRoleManager _manager;

    public ReactionRoleBootStep(IReactionRoleDomain domain, ReactionRoleManager manager)
    {
        _domain = domain;
        _manager = manager;
    }

    public async Task BootAsync()
    {
        var roles = await _domain.RetrieveAllReactionRoleDatasAsync();
        _manager.ReactionRoles = roles.ToList();
    }

    public BootOrder StepPosition => BootOrder.Async;
}