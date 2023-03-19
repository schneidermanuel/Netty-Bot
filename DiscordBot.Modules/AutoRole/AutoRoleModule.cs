using Autofac;
using DiscordBot.Framework.Contract.Modularity;
using DiscordBot.Framework.Contract.Modules.AutoRole;
using DiscordBot.Framework.Contract.TimedAction;

namespace DiscordBot.Modules.AutoRole;

internal class AutoRoleModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<AutoRoleCommands>().As<ICommandModule>();
        builder.RegisterType<AutoRoleManager>().SingleInstance().AsSelf();
        builder.RegisterType<SetupAutoRolesTask>().As<ITimedAction>();
        builder.RegisterType<AutoRoleRefresher>().As<IAutoRoleRefresher>();
    }
}