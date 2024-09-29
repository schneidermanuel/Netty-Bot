using Autofac;
using DiscordBot.DataAccess.Contract.Tournaments.Domain;
using DiscordBot.DataAccess.Modules.Tournaments.Domain;
using DiscordBot.DataAccess.Modules.Tournaments.Repository;

namespace DiscordBot.DataAccess.Modules.Tournaments;

internal class TournamentsDataAccessModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        builder.RegisterType<TournamentRepository>().As<ITournamentRepository>();
        builder.RegisterType<TournamentsDomain>().As<ITournamentsDomain>();
    }
}