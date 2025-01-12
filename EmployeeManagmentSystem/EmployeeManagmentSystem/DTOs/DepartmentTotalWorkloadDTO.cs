namespace EmployeeManagmentSystem.DTOs;

public class DepartmentTotalWorkloadDTO
{
	public required string DepartmentName { get; init; }
	
	public required int TotalHours { get; init; }
}