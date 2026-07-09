namespace GigBartending.Api.DTOs;

public class ShiftRequestSummaryDto
{
    public required string BartenderId { get; set; }
    public string? BartenderName { get; set; }
    public required string Status { get; set; }
}
