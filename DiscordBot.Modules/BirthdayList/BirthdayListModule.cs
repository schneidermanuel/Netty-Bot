using Autofac;
using DiscordBot.Framework.Contract.Modularity;
using DiscordBot.Framework.Contract.TimedAction;

namespace DiscordBot.Modules.BirthdayList;

public class BirthdayListModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);
        builder.RegisterType<BirthdayCommands>().As<ICommandModule>();
        builder.RegisterType<BirthdayListManager>().SingleInstance();
        builder.RegisterType<BirthdayListTask>().As<ITimedAction>();
    }
}