using System.Threading.Tasks;
using DiscordBot.Framework.Contract.Boot;

namespace DiscordBot.Modules.Event;

internal class EventBootstep : IBootStep
{
    private readonly EventLifesycleManager _manager;

    public EventBootstep(EventLifesycleManager manager)
    {
        _manager = manager;
    }
    public async Task BootAsync()
    {
        _manager.StartListenForChanges();
    }

    public BootOrder StepPosition => BootOrder.End;
}