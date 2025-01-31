using Logger.Enums;

namespace Logger.Models;

public class LogMessage
{
	public static string DateTimeFormat { get; set; } = "MM/dd/yyyy HH:mm:ss.fff";

	public required string MessageId { get; init; } = Guid.NewGuid().ToString();

	public string? CorrelationId { get; init; }
	
	public required LogLevel LogLevel { get; init; }

	public string LogTime => DateTime.Now.ToString(DateTimeFormat);
	
	public required string Message { get; init; }

	public string? ErrorMessage { get; init; }
	
	public required string LogWriterName { get; init; }
	
	public Dictionary<string, string>? LogKeyToLogValueMap { get; init; }
}