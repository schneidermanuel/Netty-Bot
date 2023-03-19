using System.Threading.Tasks;

namespace DiscordBot.Framework.Contract.Modules.ReactionRoles;

public interface IReactionRoleRefresher
{
    Task RefreshAsync(ulong guildId);
}