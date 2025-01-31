using Logger.Enums;

namespace Logger;

public interface ILogStrategy
{
	Task Log(LogLevel logLevel, string message, Exception? ex = null);
}