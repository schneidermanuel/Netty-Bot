using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.DataAccess.Contract.AutoMod.Violation;

namespace DiscordBot.Modules.AutoMod.KeyValueValidationStrategies;

internal class ActionKeyValueValidationStrategy : IKeyValueValidationStrategy
{
    private string[] _keys = new[]
    {
        ValidationHelper.DoNothingKey,
        ValidationHelper.DeleteMessageKey,
        ValidationHelper.DeleteAndNotifyKey,
        ValidationHelper.WarnUserKey
    };

    public bool IsResponsible(ConfigurationValueType type)
    {
        return type == ConfigurationValueType.ActionValue;
    }

    public async Task ExecuteAsync(string module, string key, string value, SocketSlashCommand context)
    {
        if (!_keys.Contains(value))
        {
            await context.RespondAsync("Der Wert ist kein gültiger Schlüssel.");
            throw new ArgumentException();
        }
    }
}