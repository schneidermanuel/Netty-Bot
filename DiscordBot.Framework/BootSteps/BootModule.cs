using Autofac;
using DiscordBot.Framework.Contract.Boot;

namespace DiscordBot.Framework.BootSteps;

public class BootModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<ConfigurationBootStep>().As<IBootStep>();
    }
}