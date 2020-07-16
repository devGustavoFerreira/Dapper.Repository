using Microsoft.Extensions.DependencyInjection;
using DapperRepository;
using Microsoft.Extensions.Configuration;
using System.IO;
using DapperRepository.Providers;

namespace Demonstration
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = ConfigureServices();

            var serviceProvider = services.BuildServiceProvider();

            // calls the Run method in App, which is replacing Main
            serviceProvider.GetService<Application>().Run();
        }

        private static IServiceCollection ConfigureServices()
        {
            IServiceCollection services = new ServiceCollection();

            var providerName = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                              .Build()
                              .GetValue<string>("Database:ProviderName");
            if (providerName.Equals("SqlServer"))
                services.AddSingleton<IProvider,DapperRepository.Providers.SqlServer.SqlServerProvider>();
            else if (providerName.Equals("MySql"))
                services.AddSingleton<IProvider,DapperRepository.Providers.MySql.MySqlProvider>();
            services.AddSingleton<IDataContext,DataContext>();
            services.AddSingleton<IRepository<Customers>, Repository<Customers>>( );

            // required to run the application
            services.AddTransient<Application>();

            return services;
        }

    }
}
