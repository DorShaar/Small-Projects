namespace EmployeeManagmentSystem.DTOs;

public class DepartmentToEmployeesDTO
{
	public required string DepartmentName { get; init; }
	
	public required IEnumerable<EmployeeDTO> Employees { get; init; }
}