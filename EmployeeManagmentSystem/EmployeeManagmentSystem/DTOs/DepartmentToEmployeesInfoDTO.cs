namespace EmployeeManagmentSystem.DTOs;

public class DepartmentToEmployeesInfoDTO
{
	public required string DepartmentName { get; init; }
	
	public required IEnumerable<EmployeeInfoDTO> EmployeesInfo { get; init; }
}