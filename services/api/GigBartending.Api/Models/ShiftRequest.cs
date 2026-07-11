namespace GigBartending.Api.Models;

public class ShiftRequest
{
    public int Id { get; set; }
    public int ShiftId { get; set; }
    public required string BartenderId { get; set; }
    public required string Status { get; set; } // "Pending", "Accepted", "Rejected"
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Shift? Shift { get; set; }
    public ApplicationUser? Bartender { get; set; }
}
