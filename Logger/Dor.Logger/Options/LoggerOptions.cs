using Logger.Enums;

namespace Logger.Options;

public class LoggerOptions
{
	public int LogsBulkSize { get; set; }
	
	public LogLevel LogLevel { get; set; }
}