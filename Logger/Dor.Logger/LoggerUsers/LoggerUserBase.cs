using Logger.Extension;

namespace Logger.LoggerUsers;

public abstract class LoggerUserBase
{
	private readonly ILogger mLogger;
	
	protected LoggerUserBase(ILogger logger)
	{
		mLogger = logger;
	}

	public async Task CreateLogs(int logsCount)
	{
		for (int i = 0; i < logsCount; ++i)
		{
			await mLogger.Debug($"message number {i}");
		}
		
		await mLogger.Info($"Done writing {logsCount} logs");
	}
}