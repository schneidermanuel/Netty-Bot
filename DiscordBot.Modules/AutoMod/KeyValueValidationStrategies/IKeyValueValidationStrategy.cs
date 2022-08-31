using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract.AutoMod.Violation;
using DiscordBot.Modules.AutoMod.Rules;

namespace DiscordBot.Modules.AutoMod.KeyValueValidationStrategies;

public interface IKeyValueValidationStrategy
{
    bool IsResponsible(ConfigurationValueType type);
    Task ExecuteAsync(string module, string key, string value, SocketSlashCommand context);
}