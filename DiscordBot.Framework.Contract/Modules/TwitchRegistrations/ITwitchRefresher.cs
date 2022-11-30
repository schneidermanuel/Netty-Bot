using System.Threading.Tasks;

namespace DiscordBot.Framework.Contract.Modules.TwitchRegistrations;

public interface ITwitchRefresher
{
    Task RefreshAsync(ulong guildId);
}