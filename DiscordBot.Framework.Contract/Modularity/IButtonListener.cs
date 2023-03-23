using System.Threading.Tasks;

namespace DiscordBot.Framework.Contract.Modularity;

public interface IButtonListener
{
    string ButtonEventPrefix { get; }
    Task ButtonPressedAsync(ulong userId, ulong messageId, ulong channelId, string customRewardId);
}