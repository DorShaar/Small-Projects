namespace EmployeeManagmentSystem.DTOs;

public class EmployeeDTO
{
	public required int EmployeeId { get; init; }
	
	public required string EmployeeName { get; init; }
	
	public required DateTime DateOfJoining { get; init; }
	
	public required string DepartmentName { get; init; }
}