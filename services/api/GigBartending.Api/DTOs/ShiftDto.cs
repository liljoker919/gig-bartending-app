namespace GigBartending.Api.DTOs;

public class ShiftDto
{
    public int Id { get; set; }
    public required string VenueId { get; set; }
    public string? VenueName { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public DateTime ShiftDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public decimal HourlyRate { get; set; }
    public required string Location { get; set; }
    public required string Status { get; set; }
    public List<string> RequestedBy { get; set; } = new();
    public List<ShiftRequestSummaryDto> Requests { get; set; } = new();
    public string? AcceptedBy { get; set; }
    public string? AcceptedByName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
