using Autofac;
using DiscordBot.Framework.Contract.Modularity;
using DiscordBot.Framework.Contract.TimedAction;

namespace DiscordBot.Modules.MusicPlayer;

public class MusicPlayerModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);
        builder.RegisterType<MusicPlayerCommands>().As<IGuildModule>();
        builder.RegisterType<MusicBootStep>().As<ITimedAction>();
        builder.RegisterType<RestartMusicPlayerTask>().As<ITimedAction>();
        builder.RegisterType<MusicManager>().SingleInstance().AsSelf();
    }
}