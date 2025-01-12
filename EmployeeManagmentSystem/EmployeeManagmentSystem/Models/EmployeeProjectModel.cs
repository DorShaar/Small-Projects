namespace EmployeeManagementSystem.Models;

public class EmployeeProjectModel
{
	public const int WorkHoursPerDay = 8;
	
	public required int EmployeeId { get; init; }
	
	public required int ProjectId { get; init; }
	
	public required DateTime StartDate { get; init; }
	
	public required DateTime EndDate { get; init; }

	public int CalculateTotalWorkLoad()
	{
		return Enumerable
			   .Range(0, (EndDate - StartDate).Days + 1)
			   .Select(i => StartDate.AddDays(i))
			   .Count(date => date.DayOfWeek != DayOfWeek.Friday && date.DayOfWeek != DayOfWeek.Saturday) * WorkHoursPerDay;
	}
}