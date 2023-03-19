using Autofac;

namespace DiscordBot.MusicPlayer;

public class MusicPlayerModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);
        builder.RegisterType<PlaylistManager>().SingleInstance();

    }
}