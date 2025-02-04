namespace Logger.Options;

public class FileLoggerOptions : LoggerOptions
{
	public int RollingFileSize { get; init; } = 100 * 1024 * 1024; // 100 MB.
	
	/// <summary>
	/// The maximal rolling files to save.
	/// </summary>
	public int RollingFileMaxGeneration { get; init; } = 3;
}