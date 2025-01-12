namespace EmployeeManagmentSystem.DTOs;

public class ProjectTotalWorkloadDTO
{
	public required string ProjectName { get; init; }
	
	public required int TotalHours { get; init; }
}