using System;
using System.Threading.Tasks;
using Discord.Commands;
using DiscordBot.Modules.AutoMod.Rules;

namespace DiscordBot.Modules.AutoMod.KeyValueValidationStrategies;

internal class AnyValueKeyValueValidationStrategy : IKeyValueValidationStrategy
{
    public bool IsResponsible(ConfigurationValueType type)
    {
        return type == ConfigurationValueType.AnyValue;
    }

    public async Task ExecuteAsync(string module, string key, string value, ICommandContext context)
    {
        if (string.IsNullOrEmpty(key))
        {
            await context.Channel.SendMessageAsync(
                $"Der Wert '{key}' muss für die Regel '{module}' eine Positive Zahl sein.");
            throw new ArgumentException();
        }
    }
}