using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using DiscordBot.Framework.Contract.Boot;
using DiscordBot.Framework.Contract.Modularity;

namespace DiscordBot.Framework.Modals;

internal class ModalManager : IBootStep
{
    private readonly DiscordSocketClient _client;
    private readonly IEnumerable<IModalListener> _listeners;

    public ModalManager(
        DiscordSocketClient client,
        IEnumerable<IModalListener> listeners)
    {
        _client = client;
        _listeners = listeners;
    }

    public async Task BootAsync()
    {
        _client.ModalSubmitted += ModalSubmitted;
        await Task.CompletedTask;
    }

    private async Task ModalSubmitted(SocketModal modal)
    {
        var prefix = modal.Data.CustomId.Split('_').First();
        var responsibleListeners = _listeners.Where(listener => listener.ButtonEventPrefix == prefix).ToArray();
        var tasks = responsibleListeners.Select(listener => listener.SubmittedAsync(modal.User.Id, modal));
        await Task.WhenAll(tasks);
    }

    public BootOrder StepPosition => BootOrder.End;
}