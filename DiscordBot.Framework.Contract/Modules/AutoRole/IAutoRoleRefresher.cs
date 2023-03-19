using System.Threading.Tasks;

namespace DiscordBot.Framework.Contract.Modules.AutoRole;

public interface IAutoRoleRefresher
{
    Task RefreshAsync(ulong guildId);
}