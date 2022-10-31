using Autofac;
using DiscordBot.Framework.Contract.TimedAction;

namespace DiscordBot.Framework.BootSteps;

public class BootModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<GuidGenerationAction>().As<ITimedAction>();
    }
}