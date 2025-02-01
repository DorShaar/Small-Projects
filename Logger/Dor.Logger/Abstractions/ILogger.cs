using Logger.Enums;

namespace Logger;

public interface ILogger : IDisposable
{
	Task Log(LogLevel logLevel, string message, Exception? ex = null);
	
	Task Flush();
	
	void SetCorrelationId(string correlationId);
}