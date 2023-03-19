using System;
using NHibernate;
using NHibernate.Cfg;

namespace DiscordBot.DataAccess.NHibernate;

public class SessionProvider : ISessionProvider
{
    private readonly ISessionFactory _sessionFactory;

    public SessionProvider()
    {
        try
        {
            _sessionFactory =
                new Configuration().AddAssembly("DiscordBot.DataAccess").Configure().BuildSessionFactory();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public ISession OpenSession() => _sessionFactory.OpenSession();
}