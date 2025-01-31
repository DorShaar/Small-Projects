using System.Collections.Concurrent;
using Logger.Enums;
using Logger.Models;
using Logger.Options;

namespace Logger;

// TODO DOR - read from configuraion the default log level and the log type (file, type)
public class Logger<T> : ILogger
{
	private readonly ILogStrategy mLogStrategy;
	private readonly LoggerOptions mLoggerOptions;
	private readonly ConcurrentDictionary<string, LogMessage> mLogMessages = new();
	private readonly SemaphoreSlim mPublishLogsLock = new(1, 1); // TODO DOR - make it thread safe
	private string? mCorrelationId;

	public Logger(ILogStrategy logStrategy, LoggerOptions loggerOptions)
	{
		mLogStrategy = logStrategy ?? throw new NullReferenceException($"{nameof(logStrategy)} is not initialized");
		mLoggerOptions = loggerOptions ?? throw new NullReferenceException($"{nameof(loggerOptions)} is not initialized");
	}
	
	public Task Log(LogLevel logLevel, string message, Exception? ex = null)
	{
		LogMessage logMessage = new()
		{
			Message = message,
			CorrelationId = 
		}
		mLogMessages.AddOrUpdate()
	}

	public async Task Flush()
	{
		await PublishLogs().ConfigureAwait(false);
	}

	public void Dispose()
	{
		Flush().Wait();
		mPublishLogsLock.Dispose();
	}

	private async Task PublishLogs()
	{
		await mLogStrategy.Log()
	}
}