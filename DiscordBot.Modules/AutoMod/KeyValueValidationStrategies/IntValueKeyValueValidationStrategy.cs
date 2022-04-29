using System;
using System.Threading.Tasks;
using Discord.Commands;
using DiscordBot.DataAccess.Contract.AutoMod.Violation;
using DiscordBot.Modules.AutoMod.Rules;

namespace DiscordBot.Modules.AutoMod.KeyValueValidationStrategies;

internal class IntValueKeyValueValidationStrategy : IKeyValueValidationStrategy
{
    public bool IsResponsible(ConfigurationValueType type)
    {
        return type == ConfigurationValueType.IntValueOnly;
    }

    public async Task ExecuteAsync(string module, string key, string value, ICommandContext context)
    {
        if (!int.TryParse(value.ToLower(), out var result))
        {
            await context.Channel.SendMessageAsync(
                $"Der Wert '{key}' muss für die Regel '{module}' eine Ganzzahl sein.");
            throw new ArgumentException();
        }

        if (result < 0)
        {
            await context.Channel.SendMessageAsync(
                $"Der Wert '{key}' muss für die Regel '{module}' eine Positive Zahl sein.");
            throw new ArgumentException();
        }
    }
}