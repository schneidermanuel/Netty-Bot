using Autofac;

namespace DiscordBot.PubSub.Backend;

public class DiscordBotPubSubBackendModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<DiscordBotPubSubBackendManager>().As<IDiscordBotPubSubBackendManager>();
    }
}