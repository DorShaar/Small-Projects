using Logger.Options;
using Microsoft.Extensions.Options;

namespace Logger;

public class LoggerFactory : ILoggerFactory
{
	private readonly ILogStrategy mLogStrategy;
	private readonly IOptionsMonitor<LoggerOptions> mLoggerOptions;

	public LoggerFactory(ILogStrategy logStrategy, IOptionsMonitor<LoggerOptions> loggerOptions)
	{
		mLogStrategy = logStrategy ?? throw new NullReferenceException($"{nameof(logStrategy)} is not initialized");
		mLoggerOptions = loggerOptions ?? throw new NullReferenceException($"{nameof(loggerOptions)} is not initialized");
	}
	
	public Logger<T> CreateLogger<T>()
	{
		return new Logger<T>(mLogStrategy, mLoggerOptions);
	}
}