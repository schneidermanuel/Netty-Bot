using Autofac;
using DiscordBot.DataAccess.Contract.TwitterRegistration.Domain;
using DiscordBot.DataAccess.Modules.TwitterRegistration.Domain;
using DiscordBot.DataAccess.Modules.TwitterRegistration.Repository;

namespace DiscordBot.DataAccess.Modules.TwitterRegistration;

internal class TwitterRegistrationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);
        builder.RegisterType<TwitterRegistrationDomain>().As<ITwitterRegistrationDomain>();
        builder.RegisterType<TwitterRegistrationRepository>().As<ITwitterRegistrationRepository>();
    }
}