using Autofac;
using DiscordBot.DataAccess.Contract.TwitterRegistration.BusinessLogic;
using DiscordBot.DataAccess.Modules.TwitterRegistration.BusinessLogic;
using DiscordBot.DataAccess.Modules.TwitterRegistration.Repository;

namespace DiscordBot.DataAccess.Modules.TwitterRegistration;

internal class TwitterRegistrationModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);
        builder.RegisterType<TwitterRegistrationBusinessLogic>().As<ITwitterRegistrationBusinessLogic>();
        builder.RegisterType<TwitterRegistrationRepository>().As<ITwitterRegistrationRepository>();
    }
}