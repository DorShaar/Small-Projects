namespace EmployeeManagementSystem.Models;

public class DepartmentModel
{
	public int Id { get; set; }

	public required string Name { get; init; }

	public ICollection<EmployeeModel> Employees { get; set; } = null!;
}