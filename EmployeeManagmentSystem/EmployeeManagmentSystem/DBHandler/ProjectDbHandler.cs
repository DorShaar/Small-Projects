using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using EmployeeManagmentSystem.DTOs;
using Microsoft.EntityFrameworkCore;

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

	/// <summary>
	/// List the top 3 projects with the highest total hours worked.
	/// </summary>
	public IEnumerable<ProjectTotalWorkloadDTO> ListTopThreeHighestWorkloadProjects()
	{
		return mContext.Projects
					   .Select(project => new ProjectTotalWorkloadDTO
					   {
						   ProjectName = project.Name,
						   TotalHours = mContext.EmployeeProjects
												.Where(employeeProject => project.Id == employeeProject.ProjectId)
												.Sum(employeeProject =>
														 (EF.Functions.DateDiffDay(employeeProject.StartDate, employeeProject.EndDate) + 1) * 8          // Assume 8-hour workdays
														 - ((EF.Functions.DateDiffDay(employeeProject.StartDate, employeeProject.EndDate) + 1) / 7) * 16 // Subtract weekends
													)
					   })
					   .OrderByDescending(projectTotalWorkload => projectTotalWorkload.TotalHours)
					   .Take(3)
					   .AsEnumerable();
	}
	
	/// <summary>
	/// For each project, list employees who worked on it, along with the hours they contributed.
	/// </summary>
	public IEnumerable<ProjectWithEmployeesWorkloadDTO> GetProjectsWithContributorsEmployees()
	{
		// First solution.
		// return mContext.Projects
		// 			   .Select(project => new ProjectWithEmployeesWorkloadDTO
		// 			   {
		// 				   ProjectName = project.Name,
		// 				   EmployeesProjectWorkload = mContext.EmployeeProjects
		// 													  .Where(employeeProject => employeeProject.ProjectId == project.Id)
		// 													  .Select(employeeProject => new EmployeeProjectWorkloadDTO
		// 													  {
		// 														  ProjectName = project.Name,
		// 														  EmployeeName = mContext.Employees.First(employee => employee.Id == employeeProject.EmployeeId).Name,
		// 														  TotalHours = (EF.Functions.DateDiffDay(employeeProject.StartDate, employeeProject.EndDate) + 1) * 8           // Assume 8-hour workdays
		// 																	   - ((EF.Functions.DateDiffDay(employeeProject.StartDate, employeeProject.EndDate) + 1) / 7) * 16 // Subtract weekends
		// 													  })
		// 													  .AsEnumerable()
		//
		// 			   })
		// 			   .AsEnumerable();
		
		// Second solution, using Navigation properties.
		return mContext.Projects
					   .Select(project => new ProjectWithEmployeesWorkloadDTO
					   {
						   ProjectName = project.Name,
						   EmployeesProjectWorkload = project.EmployeeProjectModel
															  .Where(employeeProject => employeeProject.ProjectId == project.Id)
															  .Select(employeeProject => new EmployeeProjectWorkloadDTO
															  {
																  ProjectName = project.Name,
																  EmployeeName = employeeProject.Employee.Name,
																  TotalHours = (EF.Functions.DateDiffDay(employeeProject.StartDate, employeeProject.EndDate) + 1) * 8          // Assume 8-hour workdays
																			   - ((EF.Functions.DateDiffDay(employeeProject.StartDate, employeeProject.EndDate) + 1) / 7) * 16 // Subtract weekends
															  })
															  .AsEnumerable()
					   })
					   .AsEnumerable();
	}
}