using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using EmployeeManagmentSystem.DTOs;
using Microsoft.EntityFrameworkCore;

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

	/// <summary>
	/// Fetch all employees grouped by their department, including department names. 
	/// </summary>
	public IEnumerable<DepartmentToEmployeesDTO> ListEmployeesByDepartment()
	{
		return mContext.Departments
					   .Select(department => new DepartmentToEmployeesDTO
					   {
						   DepartmentName = department.Name,
						   Employees = mContext.Employees
											   .Where(employee => employee.DepartmentId == department.Id)
											   .Select(employee => new EmployeeDTO
						   {
							   EmployeeId = employee.Id,
							   EmployeeName = employee.Name,
							   DateOfJoining = employee.DateOfJoining,
							   DepartmentName = department.Name
						   }).ToList()
					   }).ToList();
	}
	
	/// <summary>
	/// Retrieve employees who have been with the company for more than 5 years.
	/// Displays their id, name ,date of joining and department.  
	/// </summary>
	public IEnumerable<EmployeeDTO> ListLongTimeEmployees()
	{
		DateTime fiveYearsAgo = DateTime.UtcNow.AddYears(-5);
		
		return mContext.Employees
					   .Where(employee => employee.DateOfJoining <= fiveYearsAgo)
					   .Include(employee => employee.Department)
					   .Select(employee => new EmployeeDTO
								   {
									   EmployeeId = employee.Id,
									   EmployeeName = employee.Name,
									   DateOfJoining = employee.DateOfJoining,
									   DepartmentName = employee.Department.Name
								   })
					   .ToList();
	}

	/// <summary>
	/// Find employees who are not assigned to any project.
	/// </summary>
	public IEnumerable<EmployeeDTO> ListEmployeesWithoutProjects()
	{
		return mContext.Employees
					   .Where(employee => !mContext.EmployeeProjects.Any(employeeProject => employeeProject.EmployeeId == employee.Id))
					   .Select(employee => new EmployeeDTO
					   {
						   EmployeeId = employee.Id,
						   EmployeeName = employee.Name,
						   DateOfJoining = employee.DateOfJoining,
						   DepartmentName = employee.Department.Name
					   })
					   .ToList();
	}
}