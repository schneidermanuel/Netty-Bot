using Autofac;
using DiscordBot.Framework.BootSteps;

namespace DiscordBot.Framework;

public class FrameworkModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterModule<BootModule>();
    }
}