using Autofac;
using DiscordBot.DataAccess.Contract.Event;
using DiscordBot.DataAccess.Modules.Event.Domain;
using DiscordBot.DataAccess.Modules.Event.Repository;

namespace DiscordBot.DataAccess.Modules.Event;

internal class EventModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterType<EventDomain>().As<IEventDomain>();
        builder.RegisterType<EventRepository>().As<IEventRepository>();
    }
}