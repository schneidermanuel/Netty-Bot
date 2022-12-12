using System.Threading.Tasks;

namespace DiscordBot.Framework.Contract.Modules.Twitter;

public interface ITwitterRefresher
{
    Task RefreshTwitterAsync(ulong guildId);
}