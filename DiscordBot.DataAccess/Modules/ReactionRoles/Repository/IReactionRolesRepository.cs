using System.Collections.Generic;
using System.Threading.Tasks;
using DiscordBot.DataAccess.Modules.ReactionRoles.BusinessLogic;

namespace DiscordBot.DataAccess.Modules.ReactionRoles.Repository;

public interface IReactionRolesRepository
{
    Task<IEnumerable<ReactionRoleData>> RetrieveAllReactionRoleDatasAsync();
    Task SaveReactionRoleAsync(ReactionRoleData data);
    Task DeleteReactionRoleAsync(long reactionRoleId);
}