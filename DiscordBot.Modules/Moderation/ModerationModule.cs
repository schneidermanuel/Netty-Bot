using Autofac;
using DiscordBot.Framework.Contract.Modularity;

namespace DiscordBot.Modules.Moderation;

internal class ModerationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterType<ModerationCommands>().As<ICommandModule>();
    }
}