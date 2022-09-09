using Autofac;
using DiscordBot.Framework.Contract;
using DiscordBot.Framework.Contract.Modularity;
using DiscordBot.Framework.Contract.TimedAction;
using DiscordBot.Modules.TwitterNotification.TweetDataExtraction;
using TwitterSharp.Client;

namespace DiscordBot.Modules.TwitterNotification;

internal class TwitterModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        var client = new TwitterClient(BotClientConstants.TwitterBearerToken);
        builder.RegisterInstance(client).As<TwitterClient>();
        builder.RegisterType<TwitterStreamManager>().SingleInstance();
        builder.RegisterType<TwitterRegistrationCommands>().As<ICommandModule>();
        builder.RegisterType<TwitterApiInitializer>().As<ITimedAction>();
        builder.RegisterType<TwitterRuleValidator>().AsSelf();

        builder.RegisterType<AnswerExtractionStrategy>().As<IExtractionStrategy>();
        builder.RegisterType<NormalExtractionStrategy>().As<IExtractionStrategy>();
        builder.RegisterType<RetweetExtractionStrategy>().As<IExtractionStrategy>();
    }
}