using Logger.Models;

namespace Logger.LogStrategies;

public class FileLoggerStrategy : ILogStrategy
{
	public Task Log(LogMessage logMessage)
	{
		// TODO DOR
		throw new NotImplementedException();
	}

	public Task Log(IEnumerable<LogMessage> logMessages)
	{
		// TODO DOR
		throw new NotImplementedException();
	}
}