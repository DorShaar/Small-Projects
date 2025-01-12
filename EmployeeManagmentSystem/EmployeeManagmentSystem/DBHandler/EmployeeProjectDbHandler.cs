using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using EmployeeManagmentSystem.DTOs;
using Microsoft.EntityFrameworkCore.Internal;

namespace EmployeeManagmentSystem.DBHandler;

public class EmployeeProjectDbHandler : DbHandlerBase
{
	public EmployeeProjectDbHandler(EmployeeManagementContext context, ILogger<EmployeeManagementDB> logger) : base(context, logger)
	{
	}

	public async Task CreateNewEmployeeProject(int employeeId,
											   int projectId,
											   DateTime startDate,
											   DateTime endDate)
	{
		EmployeeProjectModel employeeProject = new()
		{
			EmployeeId = employeeId,
			ProjectId = projectId,
			StartDate = startDate,
			EndDate = endDate,
		};
		await mContext.EmployeeProjects.AddAsync(employeeProject).ConfigureAwait(false);
	}
}