using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using EmployeeManagmentSystem.DTOs;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagmentSystem.DBHandler;

public class DepartmentDbHandler : DbHandlerBase
{
	private readonly int[] mWeekdays =
	[
		(int)DayOfWeek.Sunday,
		(int)DayOfWeek.Monday, 
		(int)DayOfWeek.Tuesday, 
		(int)DayOfWeek.Wednesday, 
		(int)DayOfWeek.Thursday, 
	];
	
	public DepartmentDbHandler(EmployeeManagementContext context, ILogger<EmployeeManagementDB> logger) : base(context, logger)
	{
	}
	
	public async Task CreateNewDepartment(string departmentName)
	{
		DepartmentModel department = new() { Name = departmentName };
		await mContext.Departments.AddAsync(department).ConfigureAwait(false);
	}

	/// <summary>
	/// For each department, calculate the total hours worked by its employees on all projects.
	/// </summary>
	public IEnumerable<DepartmentTotalWorkloadDTO> CalculateTotalWorkloadsPerDepartment()
	{
		// Non SQL-like way.
		// List<DepartmentTotalWorkloadDTO> totalWorkloads = new();
		//
		// foreach (DepartmentModel department in mContext.Departments)
		// {
		// 	int totalDepartmentHours = 0;
		// 	foreach (EmployeeProjectModel employeeProject in mContext.EmployeeProjects
		// 															 .Where(employeeProject => mContext.Employees
		// 																							   .Where(employee => employee.DepartmentId == department.Id)
		// 																							   .Select(employee => employee.Id)
		// 																							   .Contains(employeeProject.EmployeeId)))
		// 	{
		// 		totalDepartmentHours += employeeProject.CalculateTotalWorkLoad();
		// 	}
		//
		// 	DepartmentTotalWorkloadDTO totalDepartmentWorkload = new()
		// 	{
		// 		DepartmentName = department.Name,
		// 		TotalHours = totalDepartmentHours
		// 	};
		// 	
		// 	totalWorkloads.Add(totalDepartmentWorkload);
		// }
		//
		// return totalWorkloads;
		
		return mContext.Departments
					   .Select(department => new DepartmentTotalWorkloadDTO
					   {
						   DepartmentName = department.Name,
						   TotalHours = mContext.EmployeeProjects
												.Where(employeeProject => department.Employees
																					.Select(employee => employee.Id)
																					.Contains(employeeProject.EmployeeId))
												.Sum(employeeProject =>
														 (EF.Functions.DateDiffDay(employeeProject.StartDate, employeeProject.EndDate) + 1) * 8          // Assume 8-hour workdays
														 - ((EF.Functions.DateDiffDay(employeeProject.StartDate, employeeProject.EndDate) + 1) / 7) * 16 // Subtract weekends
													)
					   })
					   .ToList();
	}
}