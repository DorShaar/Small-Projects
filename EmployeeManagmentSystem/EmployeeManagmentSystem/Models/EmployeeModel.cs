namespace EmployeeManagementSystem.Models;

public class EmployeeModel
{
	public int Id { get; set; }

	public required string Name { get; init; }
	
	public required DateTime DateOfJoining { get; init; }
	
	public required int DepartmentId { get; init; }

	public DepartmentModel Department { get; set; } = null!;
}