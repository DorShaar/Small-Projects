using Logger;
using Logger.LoggerUsers;
using Logger.LogStrategies;
using Logger.Models;
using Logger.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

class Program
{
    public static async Task Main(string[] args)
    {
        // Set up the HostBuilder
        IHost host = CreateHostBuilder(args).Build();
        await host.StartAsync().ConfigureAwait(false);

        await RunTwoLoggers(host.Services.GetRequiredService<Logger.ILoggerFactory>()).ConfigureAwait(false);

        ILogStrategy logStrategy = host.Services.GetRequiredService<ILogStrategy>();
        MemoryLoggerStrategy? memoryLogStrategy = logStrategy as MemoryLoggerStrategy;
        if (memoryLogStrategy is null)
        {
            return;
        }

        Dictionary<string, LogMessage> x = memoryLogStrategy.Logs;
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
                   .ConfigureAppConfiguration((context, config) =>
                   {
                       // Add appsettings.json configuration
                       config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                   })
                   .ConfigureServices((context, services) =>
                   {
                       services.AddSingleton<Logger.ILoggerFactory, Logger.LoggerFactory>();
                       
                       services.Configure<LoggerOptions>(context.Configuration.GetSection("LoggerOptions"));
                       
                       configureServicesAccordingToLoggerType(context, services);

                       // Configure logging (optional, depending on your needs)
                       services.AddLogging(builder =>
                       {
                           builder.AddConsole(); // Default to console logging
                       });
                   });
    }

    private static void configureServicesAccordingToLoggerType(HostBuilderContext context,
                                                               IServiceCollection services)
    {
        const string memoryLoggerType = "Memory"; 
        const string fileLoggerType = "File"; 
        const string cloudLoggerType = "Cloud"; 
     
        string? loggerType = context.Configuration.GetValue<string>("LoggerType");
        
        if (string.IsNullOrWhiteSpace(loggerType))
        {
            loggerType = memoryLoggerType;
        }
        
        // Conditionally bind the FileLoggerOptions if the LoggerType is "File"
        if (loggerType == fileLoggerType)
        {
            services.AddSingleton<ILogStrategy, FileLoggerStrategy>();
            services.Configure<FileLoggerOptions>(context.Configuration.GetSection("FileLoggerOptions"));
            return;
        }

        if (loggerType == memoryLoggerType)
        {
            services.AddSingleton<ILogStrategy, MemoryLoggerStrategy>();
            return;
        }
        
        if (loggerType == cloudLoggerType)
        {
            services.AddSingleton<ILogStrategy, CloudLoggerStrategy>();
        }
    }

    private static async Task RunTwoLoggers(Logger.ILoggerFactory loggerFactory)
    {
        LoggerUserA loggerUserA = new(loggerFactory.CreateLogger<LoggerUserA>());
        LoggerUserA loggerUserB = new(loggerFactory.CreateLogger<LoggerUserB>());

        Task aWork = loggerUserA.CreateLogs(10000);
        Task bWork = loggerUserB.CreateLogs(10000);
        
        await Task.WhenAll(aWork, bWork);
    }
}