using System.Diagnostics;
using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using EmployeeManagmentSystem.DBHandler;
using EmployeeManagmentSystem.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Controllers;

public class HomeController : Controller
{
	private readonly ILogger<HomeController> mLogger;
	private readonly EmployeeProjectDbHandler mEmployeeProjectDbHandler;
	private readonly EmployeeDbHandler mEmployeeDbHandler;
	private readonly DepartmentDbHandler mDepartmentDbHandler;
	private readonly ProjectDbHandler mProjectDbHandler;

	public HomeController(EmployeeProjectDbHandler employeeProjectDbHandler,
						  EmployeeDbHandler employeeDbHandler,
						  DepartmentDbHandler departmentDbHandler,
						  ProjectDbHandler projectDbHandler,
						  ILogger<HomeController> logger)
	{
		mLogger = logger;
		mEmployeeProjectDbHandler = employeeProjectDbHandler;
		mEmployeeDbHandler = employeeDbHandler;
		mDepartmentDbHandler = departmentDbHandler;
		mProjectDbHandler = projectDbHandler;
	}

	public IActionResult Privacy()
	{
		return View();
	}
	
	[HttpGet]
	public async Task<IActionResult> Index()
	{
		// Fetch employee and project data
		// IEnumerable<DepartmentToEmployeesDTO> results = mEmployeeDbHandler.ListEmployeesByDepartment();
		
		// Fetch all 5 years employees.
		// IEnumerable<EmployeeDTO> results = mEmployeeDbHandler.ListLongTimeEmployees();
		
		// For each department, calculate the total hours worked by its employees on all projects.
		// IEnumerable<DepartmentTotalWorkloadDTO> results = mDepartmentDbHandler.CalculateTotalWorkloadsPerDepartment();

		// List the top 3 projects with the highest total hours worked.
		// IEnumerable<ProjectTotalWorkloadDTO> results = mProjectDbHandler.ListTopThreeHighestWorkloadProjects();
		
		// For each project, list employees who worked on it, along with the hours they contributed.
		// IEnumerable<ProjectWithEmployeesWorkloadDTO> results = mProjectDbHandler.GetProjectsWithContributorsEmployees();
		
		// Find employees who are not assigned to any project.
		IEnumerable<EmployeeDTO> results = mEmployeeDbHandler.ListEmployeesWithoutProjects();

		ViewData["Results"] = results;
		return View();
	}
	
	[HttpPost]
	public async Task<IActionResult> CreateEntities(string employeeName)
	{
		// string projectName
		// string departmentName
		// string employeeName
		// await mDepartmentDbHandler.CreateNewDepartment(departmentName).ConfigureAwait(false);
		await mEmployeeDbHandler.CreateNewEmployee(employeeName, 2).ConfigureAwait(false);
		// await CreateNewProject(projectName).ConfigureAwait(false);

		await mDepartmentDbHandler.SaveChanges();

		// Redirect to the index page after creation
		return RedirectToAction("Index");
	}

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel
		{
			RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
		});
	}
}