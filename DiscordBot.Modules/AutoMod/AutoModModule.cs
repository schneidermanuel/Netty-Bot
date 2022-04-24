using Autofac;
using DiscordBot.Framework.Contract.Modularity;
using DiscordBot.Framework.Contract.TimedAction;
using DiscordBot.Modules.AutoMod.Rules;

namespace DiscordBot.Modules.AutoMod;

internal class AutoModModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<AutoModChecker>().As<IGuildModule>();
        builder.RegisterType<AutoModBootStep>().As<ITimedAction>();
        builder.RegisterType<AutoModManager>().SingleInstance();
        builder.RegisterType<EmoteSpamAutoModRule>().As<IGuildAutoModRule>().SingleInstance();
    }
}