using Autofac;
using DiscordBot.Framework.Contract.Boot;
using DiscordBot.Framework.Contract.Modularity;
using DiscordBot.Framework.Contract.TimedAction;

namespace DiscordBot.Modules.MusicPlayer;

public class MusicPlayerModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);
        builder.RegisterType<MusicPlayerCommands>().As<ICommandModule>();
        builder.RegisterType<MusicBootStep>().As<ITimedAction>();
        builder.RegisterType<MusicManager>().SingleInstance().AsSelf();
        builder.RegisterType<SpotifyApiManager>().AsSelf().As<ITimedAction>().SingleInstance();
    }
}