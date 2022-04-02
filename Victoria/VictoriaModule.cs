using Autofac;

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
            Port = 9898,
            Hostname = "10.50.1.188"
        };
        builder.RegisterInstance(config);
    }
}