using Autofac;
using DiscordBot.PubSub.Twitch;

namespace DiscordBot.PubSub;

public class PubSubModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterModule<TwitchPubsubModule>();
    }
}