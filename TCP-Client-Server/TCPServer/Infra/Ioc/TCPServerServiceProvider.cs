using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using TCPServerApp.App.Requests;
using TCPServerApp.Config;
using TCPServerApp.Infra.Requests;
using TCPServerApp.Infra.Server;

namespace TCPServerApp.Infra.Ioc
{
    public class TCPServerServiceProvider : IServiceProvider, IDisposable
    {
        private readonly ServiceProvider mServiceProvider;

        public TCPServerServiceProvider()
        {
            mServiceProvider = CreateServiceProvider();
        }

        private ServiceProvider CreateServiceProvider()
        {
            ServiceCollection serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<IRequestHandler, RequestHandler>();
            serviceCollection.AddSingleton(typeof(TCPServer));

            serviceCollection.AddLogging(builder =>
                builder.AddConsole().SetMinimumLevel(LogLevel.Information));

            AddConfiguration(serviceCollection);

            return serviceCollection.BuildServiceProvider();
        }

        private void AddConfiguration(IServiceCollection services)
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();

            configurationBuilder.AddJsonFile("settings.json", optional: false);

            IConfiguration configuration = configurationBuilder.Build();

            // Binds between IConfiguration to given configurtaion.
            services.Configure<ServerOptions>(configuration);
            services.AddOptions();
        }

        public object GetService(Type serviceType)
        {
            return mServiceProvider.GetRequiredService(serviceType);
        }

        public void Dispose()
        {
            mServiceProvider.Dispose();
        }
    }
}