using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;

namespace EmployeeManagmentSystem.DBHandler;

public class DepartmentDbHandler : DbHandlerBase
{
	public DepartmentDbHandler(EmployeeManagementContext context, ILogger<EmployeeManagementDB> logger) : base(context, logger)
	{
	}
	
	public async Task CreateNewDepartment(string departmentName)
	{
		DepartmentModel department = new() { Name = departmentName };
		await mContext.Departments.AddAsync(department).ConfigureAwait(false);
	}
}