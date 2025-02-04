using Logger.Models;

namespace Logger.LogStrategies;

public class MemoryLoggerStrategy : ILogStrategy
{
	public Dictionary<string, LogMessage> Logs { get; }= [];
	
	public Task Log(LogMessage logMessage)
	{
		Logs.Add(logMessage.MessageId, logMessage);
		return Task.CompletedTask;
	}

	public async Task Log(IEnumerable<LogMessage> logMessages)
	{
		foreach (LogMessage logMessage in logMessages)
		{
			await Log(logMessage);
		}
	}
}