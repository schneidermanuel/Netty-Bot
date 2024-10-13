using Autofac;
using DiscordBot.Framework.Contract.Modularity;
using DiscordBot.Framework.Contract.Modules.Tournaments;

namespace DiscordBot.Modules.Tournaments;

internal class TournamentModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterType<TournamentCommands>().As<ICommandModule>();
        builder.RegisterType<TournamentCompletionDomain>().As<ITournamentCompletionDomain>();
    }
}