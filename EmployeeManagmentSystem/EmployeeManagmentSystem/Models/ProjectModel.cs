namespace EmployeeManagementSystem.Models;

public class ProjectModel
{
	public int Id { get; set; }
	
	public required string Name { get; set; }

	public ICollection<EmployeeProjectModel> EmployeeProjectModel { get; set; } = null!;
}