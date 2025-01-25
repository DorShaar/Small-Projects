namespace EmployeeManagmentSystem.DTOs;

public class EmployeeInfoDTO
{
	public required string Name { get; init; }
	
	public required IEnumerable<ProjectTotalWorkloadDTO> ProjectsInfo { get; init; }
}