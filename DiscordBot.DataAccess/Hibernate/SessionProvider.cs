using System;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Dialect;
using Environment = System.Environment;

namespace DiscordBot.DataAccess.Hibernate;

public class SessionProvider : ISessionProvider
{
    private readonly ISessionFactory _sessionFactory;

    public SessionProvider()
    {
        try
        {
            var cfg = new Configuration();

            cfg.SetProperty(NHibernate.Cfg.Environment.ConnectionString,
                Environment.GetEnvironmentVariable("CONNECTION_STRING"));
            cfg.SetProperty(NHibernate.Cfg.Environment.ConnectionDriver,
                typeof(NHibernate.Driver.MySqlDataDriver).AssemblyQualifiedName);
            cfg.SetProperty(NHibernate.Cfg.Environment.Dialect, typeof(MySQL5Dialect).AssemblyQualifiedName);
            cfg.SetProperty(NHibernate.Cfg.Environment.ConnectionProvider,
                typeof(NHibernate.Connection.DriverConnectionProvider).AssemblyQualifiedName);
            cfg.SetProperty(NHibernate.Cfg.Environment.ShowSql, "true");

            cfg.AddAssembly("DiscordBot.DataAccess");

            _sessionFactory = cfg.BuildSessionFactory();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public ISession OpenSession() => _sessionFactory.OpenSession();
}