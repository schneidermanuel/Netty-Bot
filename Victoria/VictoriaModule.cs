using Autofac;
using DiscordBot.Framework.Contract;

namespace Victoria;

/// <summary>
/// Module for Vicroria
/// </summary>
public class VictoriaModule : Module
{
    /// <summary>
    /// Load Method
    /// </summary>
    /// <param name="builder">shit</param>
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<LavaNode>().SingleInstance().AsSelf();
        var config = new LavaConfig
        {
            Port = BotClientConstants.LavalinkPort,
            Hostname = BotClientConstants.LavalinkHost,
            Authorization = BotClientConstants.LavalinkPassword,
            IsSsl = BotClientConstants.LavalinkSsl
        };
        builder.RegisterInstance(config);
    }
}