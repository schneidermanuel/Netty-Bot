using Autofac;
using DiscordBot.PubSub.Twitch;
using DiscordBot.PubSub.Youtube;

namespace DiscordBot.PubSub;

public class PubSubModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterModule<TwitchPubsubModule>();
        builder.RegisterModule<YoutubeModule>();
    }
}