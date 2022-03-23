using Autofac;
using DiscordBot.Framework.Contract.Modularity;

namespace DiscordBot.Modules.Basics;

public class BasicModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<BasicCommandModule>().As<IGuildModule>();
    }
}