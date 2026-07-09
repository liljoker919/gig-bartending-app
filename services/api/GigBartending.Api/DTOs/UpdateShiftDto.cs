namespace GigBartending.Api.DTOs;

public class UpdateShiftDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime? ShiftDate { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    public decimal? HourlyRate { get; set; }
    public string? Location { get; set; }
}
