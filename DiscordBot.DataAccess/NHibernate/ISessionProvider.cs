using NHibernate;

namespace DiscordBot.DataAccess.NHibernate;

public interface ISessionProvider
{
    ISession OpenSession();
}