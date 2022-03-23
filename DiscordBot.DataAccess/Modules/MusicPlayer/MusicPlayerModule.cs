using Autofac;
using DiscordBot.DataAccess.Contract.MusicPlayer;
using DiscordBot.DataAccess.Modules.MusicPlayer.BusinessLogic;
using DiscordBot.DataAccess.Modules.MusicPlayer.Repository;

namespace DiscordBot.DataAccess.Modules.MusicPlayer;

public class MusicPlayerModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<MusicPlayerBusinessLogic>().As<IMusicPlayerBusinessLogic>();
        builder.RegisterType<MusicPlayerRepository>().As<IMusicPlayerRepository>();
    }
}