namespace EmployeeManagementSystem.Models;

public class EmployeeModel
{
	public int Id { get; set; }

	public required string Name { get; set; }
	
	public required DateTime DateOfJoining { get; set; }
	
	public required int DepartmentId { get; set; }
}