using Autofac;
using DiscordBot.Framework.Contract.Modularity;

namespace DiscordBot.Modules.Tournaments;

internal class TournamentModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterType<TournamentCommands>().As<ICommandModule>();
    }
}