using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordBot.DataAccess.Modules.ReactionRoles.BusinessLogic;

internal interface IReactionRolesRepository
{
    Task<IEnumerable<ReactionRoleData>> RetrieveAllReactionRoleDatasAsync();
    Task SaveReactionRoleAsync(ReactionRoleData data);
    Task DeleteReactionRoleAsync(long reactionRoleId);
}