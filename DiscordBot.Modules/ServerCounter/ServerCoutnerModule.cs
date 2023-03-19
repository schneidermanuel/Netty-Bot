using Autofac;
using DiscordBot.Framework.Contract.TimedAction;

namespace DiscordBot.Modules.ServerCounter;

internal class ServerCoutnerModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<ServerCoutnerAction>().As<ITimedAction>();
    }
}