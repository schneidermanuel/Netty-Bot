using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract.AutoMod.Violation;

namespace DiscordBot.Modules.AutoMod.KeyValueValidationStrategies;

internal class AnyValueKeyValueValidationStrategy : IKeyValueValidationStrategy
{
    public bool IsResponsible(ConfigurationValueType type)
    {
        return type == ConfigurationValueType.AnyValue;
    }

    public async Task ExecuteAsync(string module, string key, string value, SocketSlashCommand context)
    {
        if (string.IsNullOrEmpty(key))
        {
            await context.RespondAsync(
                $"Der Wert '{key}' muss für die Regel '{module}' eine Positive Zahl sein.");
            throw new ArgumentException();
        }
    }
}