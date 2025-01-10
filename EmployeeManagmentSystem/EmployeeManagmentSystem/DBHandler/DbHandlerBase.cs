using EmployeeManagementSystem.Data;

namespace EmployeeManagmentSystem.DBHandler;

public class DbHandlerBase
{
	private readonly ILogger<EmployeeManagementDB> mLogger;
	protected EmployeeManagementContext mContext;

	public DbHandlerBase(EmployeeManagementContext context, ILogger<EmployeeManagementDB> logger)
	{
		mLogger = logger;
		mContext = context;
	}
	
	public async Task SaveChanges()
	{
		mLogger.Log(LogLevel.Information, "Saving changes");
		await mContext.SaveChangesAsync().ConfigureAwait(false);
	}
}