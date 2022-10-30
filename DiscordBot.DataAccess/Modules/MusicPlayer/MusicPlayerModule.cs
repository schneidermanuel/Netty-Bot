using Autofac;
using DiscordBot.DataAccess.Contract.MusicPlayer;
using DiscordBot.DataAccess.Modules.MusicPlayer.Domain;
using DiscordBot.DataAccess.Modules.MusicPlayer.Repository;

namespace DiscordBot.DataAccess.Modules.MusicPlayer;

public class MusicPlayerModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<MusicPlayerDomain>().As<IMusicPlayerDomain>();
        builder.RegisterType<MusicPlayerRepository>().As<IMusicPlayerRepository>();
    }
}