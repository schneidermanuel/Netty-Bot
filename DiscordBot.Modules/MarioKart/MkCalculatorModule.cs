using Autofac;
using DiscordBot.Framework.Contract.Modularity;
using DiscordBot.Framework.Contract.TimedAction;

namespace DiscordBot.Modules.MarioKart;

internal class MkCalculatorModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterType<MkCalculatorCommands>().As<ICommandModule>();
        builder.RegisterType<MkCalculator>().As<IMkCalculator>();
        builder.RegisterType<MkGameManager>().AsSelf().SingleInstance();
        builder.RegisterType<MkWorldRecordMapper>().As<IMkWorldRecordMapper>();
        builder.RegisterType<MkWorldRecordLoader>().As<IMkWorldRecordLoader>();
        builder.RegisterType<MarioKartGameCloser>().As<ITimedAction>();
    }
}