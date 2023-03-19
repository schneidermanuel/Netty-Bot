using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;

namespace DiscordBot.DataAccess.Contract.ReactionRoles
{
    public interface IReactionRoleDomain
    {
        Task<IEnumerable<ReactionRole>> RetrieveAllReactionRoleDatasAsync();
        Task SaveReactionRoleAsync(ReactionRole reactionRole);
        Task DeleteReactionRoleAsync(long reactionRoleId);
        Task<bool> CanAddReactionRoleAsync(ulong messageId, IEmote emote);
        Task<IEnumerable<ReactionRole>> RetrieveReactionRolesForGuildAsync(ulong guildId);
    }
}