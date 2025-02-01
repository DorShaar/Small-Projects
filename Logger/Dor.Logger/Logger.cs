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
	private readonly SemaphoreSlim mPublishLogsLock = new (1, 1);
	private readonly string mLoggerId = Guid.NewGuid().ToString();
	private string? mCorrelationId;

	public Logger(ILogStrategy logStrategy, LoggerOptions loggerOptions)
	{
		mLogStrategy = logStrategy ?? throw new NullReferenceException($"{nameof(logStrategy)} is not initialized");
		mLoggerOptions = loggerOptions ?? throw new NullReferenceException($"{nameof(loggerOptions)} is not initialized");
	}
	
	public async Task Log(LogLevel logLevel, string message, Exception? ex = null)
	{
		try
		{
			if (logLevel < mLoggerOptions.LogLevel)
			{
				return;
			}
			
			LogMessage logMessage = new()
			{
				Message = message,
				CorrelationId = mCorrelationId,
				ErrorMessage = ex?.Message,
				LogLevel = logLevel,
				LoggerId = mLoggerId,
				LogWriterName = nameof(T)
			};
			
			await mPublishLogsLock.WaitAsync();
			
			mLogMessages.AddOrUpdate(logMessage.MessageId, logMessage, (_, _) => logMessage);

			if (mLogMessages.Count < mLoggerOptions.LogsBulkSize)
			{
				return;
			}

			await PublishLogsAndClearBuffer().ConfigureAwait(false);
		}
		finally
		{
			mPublishLogsLock.Release();
		}
	}

	public async Task Flush()
	{
		await PublishLogsAndClearBuffer().ConfigureAwait(false);
	}

	public void SetCorrelationId(string correlationId)
	{
		// TODO DOR think about tread safe.
		mCorrelationId = correlationId;
	}

	public void Dispose()
	{
		Flush().Wait();
		mPublishLogsLock.Dispose();
	}

	private async Task PublishLogsAndClearBuffer()
	{
		if (mLogMessages.Count == 0)
		{
			return;
		}
		
		if (mLogMessages.Count == 1)
		{
			await mLogStrategy.Log(mLogMessages.First().Value).ConfigureAwait(false);
			return;
		}
		
		await mLogStrategy.Log(mLogMessages.Values).ConfigureAwait(false);

		try
		{
			await mPublishLogsLock.WaitAsync();
			mLogMessages.Clear();
		}
		finally
		{
			mPublishLogsLock.Release();
		}
	}
}