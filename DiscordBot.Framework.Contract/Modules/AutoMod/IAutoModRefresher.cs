using System.Threading.Tasks;

namespace DiscordBot.Framework.Contract.Modules.AutoMod;

public interface IAutoModRefresher
{
    Task RefreshGuildAsync(ulong guildId);
}