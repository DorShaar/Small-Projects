namespace EmployeeManagmentSystem.DTOs;

public class EmployeeProjectWorkloadDTO
{
	public required string ProjectName { get; init; }
	
	public required string EmployeeName { get; init; }
	
	public required int TotalHours { get; init; }
}