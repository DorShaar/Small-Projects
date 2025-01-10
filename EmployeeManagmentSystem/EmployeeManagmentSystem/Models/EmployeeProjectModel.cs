namespace EmployeeManagementSystem.Models;

public class EmployeeProjectModel
{
	public required int EmployeeId { get; init; }
	
	public required int ProjectId { get; init; }
	
	public required DateTime StartDate { get; set; }
	
	public required DateTime EndDate { get; set; }
}