using Autofac;

namespace DiscordBot.PubSub.Twitch;

internal class TwitchPubsubModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterType<TwitchPubsubManager>().As<ITwitchPubsubManager>().SingleInstance();
    }
}