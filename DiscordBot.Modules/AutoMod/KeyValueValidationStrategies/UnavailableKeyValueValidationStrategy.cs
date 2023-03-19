using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract.AutoMod.Violation;

namespace DiscordBot.Modules.AutoMod.KeyValueValidationStrategies;

internal class UnavailableKeyValueValidationStrategy : IKeyValueValidationStrategy
{
    public bool IsResponsible(ConfigurationValueType type)
    {
        return type == ConfigurationValueType.Unavailable;
    }

    public async Task ExecuteAsync(string module, string key, string value, SocketSlashCommand context)
    {
        await context.RespondAsync(
            $"Der Wert '{key}' darf für die Regel '{module}' nicht gesetzt werden.");
        throw new ArgumentException();
    }
}