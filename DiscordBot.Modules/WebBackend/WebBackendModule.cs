using Autofac;
using DiscordBot.Framework.Contract.Boot;

namespace DiscordBot.Modules.WebBackend;

internal class WebBackendModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterType<BackendStarter>().As<IBootStep>();
    }
}