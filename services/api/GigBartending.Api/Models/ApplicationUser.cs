using Microsoft.AspNetCore.Identity;

namespace GigBartending.Api.Models;

public class ApplicationUser : IdentityUser
{
    public required string Role { get; set; } // "Bartender" or "Venue"
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<Shift>? PostedShifts { get; set; }
    public ICollection<Shift>? AcceptedShifts { get; set; }
    public ICollection<ShiftRequest>? ShiftRequests { get; set; }
}
