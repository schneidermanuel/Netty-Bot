using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract.AutoMod.Violation;
using DiscordBot.Modules.AutoMod.Rules;

namespace DiscordBot.Modules.AutoMod.KeyValueValidationStrategies;

internal class BoolValueKeyValueValidationStrategy : IKeyValueValidationStrategy
{
    public bool IsResponsible(ConfigurationValueType type)
    {
        return type == ConfigurationValueType.BoolValueOnly;
    }

    public async Task ExecuteAsync(string module, string key, string value, SocketSlashCommand context)
    {
        if (!bool.TryParse(value.ToLower(), out _))
        {
            await context.RespondAsync(
                $"Der Wert '{key}' muss für die Regel '{module}' entweder TRUE oder FALSE sein.");
            throw new ArgumentException();
        }
    }
}