using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;

namespace EmployeeManagmentSystem.DBHandler;

public class ProjectDbHandler : DbHandlerBase
{
	public ProjectDbHandler(EmployeeManagementContext context, ILogger<EmployeeManagementDB> logger) : base(context, logger)
	{
	}
	
	public async Task CreateNewProject(string projectName)
	{
		// Create a new project
		ProjectModel project = new() { Name = projectName };
		await mContext.Projects.AddAsync(project).ConfigureAwait(false);
	}
}