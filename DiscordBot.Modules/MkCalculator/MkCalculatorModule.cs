using Autofac;
using DiscordBot.Framework.Contract.Modularity;

namespace DiscordBot.Modules.MkCalculator;

internal class MkCalculatorModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterType<MkCalculatorCommands>().As<IGuildModule>();
        builder.RegisterType<MkCalculator>().As<IMkCalculator>();
        builder.RegisterType<MkManager>().AsSelf().SingleInstance();
        builder.RegisterType<MkWorldRecordMapper>().As<IMkWorldRecordMapper>();
        builder.RegisterType<MkWorldRecordLoader>().As<IMkWorldRecordLoader>();
    }
}