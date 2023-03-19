using Autofac;

namespace DiscordBot.PubSub.Youtube;

internal class YoutubeModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<YoutubePubSubRegistrator>().As<IYoutubePubSubRegistrator>();
    }
}