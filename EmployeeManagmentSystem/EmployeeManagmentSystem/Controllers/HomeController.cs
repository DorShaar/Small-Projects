using System.Diagnostics;
using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using EmployeeManagmentSystem.DBHandler;
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
		var employeeProjects = await _context.Employees
											 .Include(e => e.Department)
											 .SelectMany(e => _context.EmployeeProjects.Where(ep => ep.EmployeeId == e.Id)
																	  .Join(_context.Projects, ep => ep.ProjectId, p => p.Id, (ep, p) => new
																	  {
																		  EmployeeId = e.Id,
																		  EmployeeName = e.Name,
																		  ProjectName = p.Name
																	  }))
											 .ToListAsync();

		ViewData["EmployeeProjects"] = employeeProjects;

		return View();
	}
	
	[HttpPost]
	public async Task<IActionResult> CreateEntities(string projectName)
	{
		// await CreateNewDepartment(departmentName).ConfigureAwait(false);
		// await CreateNewEmployee(employeeName, 1).ConfigureAwait(false);
		// await CreateNewProject(projectName).ConfigureAwait(false);

		await mDb.SaveChanges();

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