using NHibernate;

namespace DiscordBot.DataAccess.NHibernate;

public interface ISessionFactoryProvider
{
    ISession OpenSession();
}