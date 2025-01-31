using Logger.Enums;

namespace Logger.Models;

public class LogMessage
{
	public required LogLevel LogLevel { get; init; }
	
	public required string Message { get; init; }

	public string? ErrorMessage { get; init; }
	
	public required string LogWriterName { get; init; }
	
	public Dictionary<string, string>? LogKeyToLogValueMap { get; init; }
}