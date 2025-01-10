using EmployeeManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.Data;

public class EmployeeManagementContext : DbContext
{
	public EmployeeManagementContext(DbContextOptions<EmployeeManagementContext> options)
		: base(options)
	{
	}
	
	public DbSet<EmployeeModel> Employees { get; set; }
	
	public DbSet<DepartmentModel> Departments { get; set; }
	
	public DbSet<ProjectModel> Projects { get; set; }
	
	public DbSet<EmployeeProjectModel> EmployeeProjects { get; set; }
	
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<EmployeeProjectModel>()
					.HasKey(employeeProject => new { employeeProject.EmployeeId, employeeProject.ProjectId });

		base.OnModelCreating(modelBuilder);
	}
}