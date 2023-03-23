using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.Framework.Contract.Boot;
using DiscordBot.Framework.Contract.Modularity;

namespace DiscordBot.Framework.Buttons;

internal class ButtonManager : IBootStep
{
    private readonly DiscordSocketClient _client;
    private readonly IEnumerable<IButtonListener> _listeners;

    public ButtonManager(DiscordSocketClient client, IEnumerable<IButtonListener> listeners)
    {
        _client = client;
        _listeners = listeners;
    }

    public async Task BootAsync()
    {
        await Task.CompletedTask;
        _client.ButtonExecuted += ButtonPressed;
    }

    private async Task ButtonPressed(SocketMessageComponent arg)
    {
        var customId = arg.Data.CustomId;
        var messageId = arg.Message.Id;
        var channelId = arg.Message.Channel.Id;
        var prefix = customId.Split('_').First();
        var listeners = _listeners.Where(l => l.ButtonEventPrefix == prefix).ToArray();
        var tasks = listeners.Select(l => l.ButtonPressedAsync(arg.User.Id, messageId, channelId, customId));
        await arg.DeferAsync();
        await Task.WhenAll(tasks);
    }

    public BootOrder StepPosition => BootOrder.End;
}