using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;

namespace DiscordBot.DataAccess.Modules.ReactionRoles.Domain;

internal interface IReactionRolesRepository
{
    Task<IEnumerable<ReactionRoleData>> RetrieveAllReactionRoleDatasAsync();
    Task SaveReactionRoleAsync(ReactionRoleData data);
    Task DeleteReactionRoleAsync(long reactionRoleId);
    Task<bool> CanAddReactionRoleAsync(string messageId, IEmote emote);
}