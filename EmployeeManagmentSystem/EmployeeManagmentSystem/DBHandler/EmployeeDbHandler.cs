using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;

namespace EmployeeManagmentSystem.DBHandler;

public class EmployeeDbHandler : DbHandlerBase
{
	public EmployeeDbHandler(EmployeeManagementContext context, ILogger<EmployeeManagementDB> logger) : base(context, logger)
	{
	}
	
	public async Task CreateNewEmployee(string employeeName, int departmentId)
	{
		EmployeeModel employee = new()
		{
			Name = employeeName,
			DepartmentId = departmentId,
			DateOfJoining = DateTime.Now,
		};
		await mContext.Employees.AddAsync(employee).ConfigureAwait(false);
	}
}