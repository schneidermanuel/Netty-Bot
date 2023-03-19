using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract.AutoMod.Violation;

namespace DiscordBot.Modules.AutoMod.KeyValueValidationStrategies;

internal class IntValueKeyValueValidationStrategy : IKeyValueValidationStrategy
{
    public bool IsResponsible(ConfigurationValueType type)
    {
        return type == ConfigurationValueType.IntValueOnly;
    }

    public async Task ExecuteAsync(string module, string key, string value, SocketSlashCommand context)
    {
        if (!int.TryParse(value.ToLower(), out var result))
        {
            await context.RespondAsync(
                $"Der Wert '{key}' muss für die Regel '{module}' eine Ganzzahl sein.");
            throw new ArgumentException();
        }

        if (result < 0)
        {
            await context.RespondAsync(
                $"Der Wert '{key}' muss für die Regel '{module}' eine Positive Zahl sein.");
            throw new ArgumentException();
        }

        if (result > 100)
        {
            await context.Channel.SendMessageAsync(
                $"Der gesetzte Wert für '{key}' ist für die Regel '{module}' zu gross.");
            throw new ArgumentException();

        }
    }
}