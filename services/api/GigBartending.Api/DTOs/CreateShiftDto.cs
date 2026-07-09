namespace GigBartending.Api.DTOs;

public class CreateShiftDto
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public DateTime ShiftDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public decimal HourlyRate { get; set; }
    public required string Location { get; set; }
}
