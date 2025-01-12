namespace EmployeeManagmentSystem.DTOs;

public class ProjectWithEmployeesWorkloadDTO
{
	public required string ProjectName { get; init; }

	public required IEnumerable<EmployeeProjectWorkloadDTO> EmployeesProjectWorkload { get; init; }
}