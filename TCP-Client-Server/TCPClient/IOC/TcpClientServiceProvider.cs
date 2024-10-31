using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TCPClient.Settings;

namespace TCPClient.IOC;

public class TcpClientServiceProvider : IServiceProvider, IDisposable
{
	private readonly ServiceProvider mServiceProvider;
	
	public TcpClientServiceProvider()
	{
		mServiceProvider = CreateServiceProvider();
	}
	
	private ServiceProvider CreateServiceProvider()
	{
		ServiceCollection serviceCollection = [];

		RegisterComponents(serviceCollection);

		RegisterLogger(serviceCollection);

		AddConfiguration(serviceCollection);

		return serviceCollection.BuildServiceProvider();
	}

	private void RegisterComponents(IServiceCollection serviceCollection)
	{
		serviceCollection.AddSingleton<TCPClientApp.TCPClient>();
	}
	
	private void RegisterLogger(IServiceCollection serviceCollection)
	{
		serviceCollection.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));
	}
        
	private void AddConfiguration(IServiceCollection services)
	{
		ConfigurationBuilder configurationBuilder = new();
		configurationBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
		IConfiguration configuration = configurationBuilder.Build();

		services.Configure<TcpClientSettings>(configuration.GetSection("TcpClientSettings"));
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