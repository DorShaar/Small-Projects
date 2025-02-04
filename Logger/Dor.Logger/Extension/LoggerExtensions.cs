using Logger.Enums;

namespace Logger.Extension;

public static class LoggerExtensions
{
	public static async Task Info(this ILogger logger, string message)
	{
		await logger.Log(LogLevel.Information, message).ConfigureAwait(false);
	}
	
	public static async Task Debug(this ILogger logger, string message)
	{
		await logger.Log(LogLevel.Debug, message).ConfigureAwait(false);
	}
}