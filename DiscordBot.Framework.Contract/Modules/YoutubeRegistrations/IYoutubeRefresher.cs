using System.Threading.Tasks;

namespace DiscordBot.Framework.Contract.Modules.YoutubeRegistrations;

public interface IYoutubeRefresher
{
    Task RefreshGuildAsync(ulong guildId);
}