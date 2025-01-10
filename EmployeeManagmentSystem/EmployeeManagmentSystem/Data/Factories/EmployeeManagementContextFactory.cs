using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EmployeeManagementSystem.Data.Factories;

public class EmployeeManagementContextFactory : IDesignTimeDbContextFactory<EmployeeManagementContext>
{
	public EmployeeManagementContext CreateDbContext(string[] args)
	{
		IConfigurationRoot configuration = new ConfigurationBuilder()
										   .SetBasePath(Directory.GetCurrentDirectory())
										   .AddJsonFile("appsettings.json")
										   .Build();

		DbContextOptionsBuilder<EmployeeManagementContext> optionsBuilder = new();
		optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

		return new EmployeeManagementContext(optionsBuilder.Options);
	}
}