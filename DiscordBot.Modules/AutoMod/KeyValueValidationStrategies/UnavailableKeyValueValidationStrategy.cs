using System;
using System.Threading.Tasks;
using Discord.Commands;
using DiscordBot.Modules.AutoMod.Rules;

namespace DiscordBot.Modules.AutoMod.KeyValueValidationStrategies;

internal class UnavailableKeyValueValidationStrategy : IKeyValueValidationStrategy
{
    public bool IsResponsible(ConfigurationValueType type)
    {
        return type == ConfigurationValueType.Unavailable;
    }

    public async Task ExecuteAsync(string module, string key, string value, ICommandContext context)
    {
        await context.Channel.SendMessageAsync(
            $"Der Wert '{key}' darf für die Regel '{module}' nicht gesetzt werden.");
        throw new ArgumentException();
    }
}