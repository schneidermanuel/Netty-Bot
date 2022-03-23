using Autofac;

namespace Victoria;

public class VictoriaModule : Module
{
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