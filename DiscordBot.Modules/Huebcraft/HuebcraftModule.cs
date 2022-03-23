using Autofac;
using DiscordBot.Framework.Contract.Modularity;

namespace DiscordBot.Modules.Huebcraft;

public class HuebcraftModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<HuebcraftCommands>().As<IGuildModule>();
    }
}