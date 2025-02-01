using Logger.Models;

namespace Logger;

public interface ILogStrategy
{
	Task Log(LogMessage logMessage);
	
	Task Log(IEnumerable<LogMessage> logMessages);
}