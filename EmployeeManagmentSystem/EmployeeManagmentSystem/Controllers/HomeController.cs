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
		// IEnumerable<DepartmentToEmployeesDTO> employeeProjects = mEmployeeDbHandler.ListEmployeesByDepartment();
		
		// Fetch all 5 years employees.
		IEnumerable<EmployeeDTO> employeeProjects = mEmployeeDbHandler.ListLongTimeEmployees();

		ViewData["Results"] = employeeProjects;

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