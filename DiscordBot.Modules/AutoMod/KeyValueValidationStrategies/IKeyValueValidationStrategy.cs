using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract.AutoMod.Violation;

namespace DiscordBot.Modules.AutoMod.KeyValueValidationStrategies;

public interface IKeyValueValidationStrategy
{
    bool IsResponsible(ConfigurationValueType type);
    Task ExecuteAsync(string module, string key, string value, SocketSlashCommand context);
}