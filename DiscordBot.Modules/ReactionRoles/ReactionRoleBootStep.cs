using System.Linq;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Contract.ReactionRoles;
using DiscordBot.Framework.Contract.Boot;

namespace DiscordBot.Modules.ReactionRoles;

public class ReactionRoleBootStep : IBootStep
{
    private readonly IReactionRoleBusinessLogic _businessLogic;
    private readonly ReactionRoleManager _manager;

    public ReactionRoleBootStep(IReactionRoleBusinessLogic businessLogic, ReactionRoleManager manager)
    {
        _businessLogic = businessLogic;
        _manager = manager;
    }

    public async Task BootAsync()
    {
        var roles = await _businessLogic.RetrieveAllReactionRoleDatasAsync();
        _manager.ReactionRoles = roles.ToList();
    }

    public BootOrder StepPosition => BootOrder.Async;
}