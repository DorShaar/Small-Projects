using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using TCPServerApp.Settings;
using TCPServerApp.Infra.Server;

namespace TCPServerApp.Infra
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
            ServiceCollection serviceCollection = new();

            serviceCollection.AddSingleton<TCPServer>();

            serviceCollection.AddLogging(builder =>
                builder.AddConsole().SetMinimumLevel(LogLevel.Information));

            AddConfiguration(serviceCollection);

            return serviceCollection.BuildServiceProvider();
        }

        private void AddConfiguration(IServiceCollection services)
        {
            ConfigurationBuilder configurationBuilder = new();
            configurationBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            IConfiguration configuration = configurationBuilder.Build();

            services.Configure<TcpServerSettings>(configuration.GetSection("TcpServerSettings"));
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