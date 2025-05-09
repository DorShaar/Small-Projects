﻿using System.Collections.Concurrent;
using Logger.Enums;
using Logger.Models;
using Logger.Options;
using Microsoft.Extensions.Options;

namespace Logger;

public class Logger<T> : ILogger
{
	private readonly ILogStrategy mLogStrategy;
	private readonly IOptionsMonitor<LoggerOptions> mLoggerOptions;
	private readonly ConcurrentQueue<LogMessage> mLogMessages = new();
	private readonly SemaphoreSlim mLogsLock = new (1, 1);
    private readonly string mLoggerId = Guid.NewGuid().ToString();

	public Logger(ILogStrategy logStrategy, IOptionsMonitor<LoggerOptions> loggerOptions)
	{
		mLogStrategy = logStrategy ?? throw new NullReferenceException($"{nameof(logStrategy)} is not initialized");
		mLoggerOptions = loggerOptions ?? throw new NullReferenceException($"{nameof(loggerOptions)} is not initialized");
	}
	
	public async Task Log(LogLevel logLevel, string message, Exception? exception = null)
	{
		if (logLevel < mLoggerOptions.CurrentValue.LogLevel)
		{
			return;
		}

        try
        {
            LogMessage logMessage = new()
			{
				Message = message,
				ErrorMessage = exception?.Message,
				LogLevel = logLevel,
				LoggerId = mLoggerId,
				LogWriterName = typeof(T).FullName ?? "Unknown"
			};
		
			if (mLogMessages.Count >= mLoggerOptions.CurrentValue.LogsBulkSize)
			{
				await PublishLogsAndClearBuffer();
			}

			mLogMessages.Enqueue(logMessage);
		}
		catch (Exception ex)
		{
			// Do some error handling.
		}
	}

	public async Task Flush()
	{
		await PublishLogsAndClearBuffer();
	}

	public void Dispose()
	{
		Flush().Wait();
		mLogsLock.Dispose();
	}

	private async Task PublishLogsAndClearBuffer()
	{
		try
		{
			await mLogsLock.WaitAsync();
			if (mLogMessages.Count == 0)
			{
				return;
			}
			
			if (mLogMessages.Count == 1)
			{
				await mLogStrategy.Log(mLogMessages.First());
				return;
			}

			while (mLogMessages.Count > 0)
			{
				mLogMessages.TryDequeue(out LogMessage? message);
				if (message is null)
				{
					continue;
				}
				
				await mLogStrategy.Log(message);
			}

			mLogMessages.Clear();
		}
		finally
		{
			mLogsLock.Release();
		}
	}
}