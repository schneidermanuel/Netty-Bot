using NHibernate;

namespace DiscordBot.DataAccess.Hibernate;

public interface ISessionProvider
{
    ISession OpenSession();
}