using Autofac;
using DiscordBot.Framework.Contract.Modularity;
using DiscordBot.Framework.Contract.TimedAction;
using DiscordBot.Modules.AutoMod.KeyValueValidationStrategies;
using DiscordBot.Modules.AutoMod.Rules;
using DiscordBot.Modules.AutoMod.Rules.Rules;

namespace DiscordBot.Modules.AutoMod;

internal class AutoModModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<AutoModChecker>().As<IGuildModule>();
        builder.RegisterType<AutoModCommands>().As<IGuildModule>();
        builder.RegisterType<AutoModBootStep>().As<ITimedAction>();
        builder.RegisterType<AutoModManager>().SingleInstance();

        builder.RegisterType<EmoteSpamAutoModRule>().As<IGuildAutoModRule>().SingleInstance();
        builder.RegisterType<CapsLockAutoModRule>().As<IGuildAutoModRule>().SingleInstance();

        builder.RegisterType<BoolValueKeyValueValidationStrategy>().As<IKeyValueValidationStrategy>();
        builder.RegisterType<UnavailableKeyValueValidationStrategy>().As<IKeyValueValidationStrategy>();
        builder.RegisterType<IntValueKeyValueValidationStrategy>().As<IKeyValueValidationStrategy>();
        builder.RegisterType<AnyValueKeyValueValidationStrategy>().As<IKeyValueValidationStrategy>();
        builder.RegisterType<ActionKeyValueValidationStrategy>().As<IKeyValueValidationStrategy>();
    }
}