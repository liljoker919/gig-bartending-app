using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GigBartending.Api.Data;
using GigBartending.Api.Models;
using GigBartending.Api.DTOs;

namespace GigBartending.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShiftsController : ControllerBase
{
    private readonly GigBartendingDbContext _context;

    public ShiftsController(GigBartendingDbContext context)
    {
        _context = context;
    }

    private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetShifts([FromQuery] string? status, [FromQuery] DateOnly? date)
    {
        var query = _context.Shifts
            .Include(s => s.Venue)
            .Include(s => s.AcceptedByUser)
            .Include(s => s.Requests).ThenInclude(r => r.Bartender)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(s => s.Status.ToLower() == status.ToLower());
        }

        if (date.HasValue)
        {
            var start = date.Value.ToDateTime(TimeOnly.MinValue);
            var end = start.AddDays(1);
            query = query.Where(s => s.ShiftDate >= start && s.ShiftDate < end);
        }

        var shifts = await query.OrderBy(s => s.ShiftDate).ToListAsync();

        return Ok(new { shifts = shifts.Select(ToDto) });
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetShift(int id)
    {
        var shift = await _context.Shifts
            .Include(s => s.Venue)
            .Include(s => s.AcceptedByUser)
            .Include(s => s.Requests).ThenInclude(r => r.Bartender)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (shift == null)
        {
            return NotFound(new { message = "Shift not found" });
        }

        return Ok(ToDto(shift));
    }

    [HttpPost]
    [Authorize(Roles = "Venue")]
    public async Task<IActionResult> CreateShift([FromBody] CreateShiftDto dto)
    {
        var shift = new Shift
        {
            VenueId = CurrentUserId,
            Title = dto.Title,
            Description = dto.Description,
            ShiftDate = dto.ShiftDate,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            HourlyRate = dto.HourlyRate,
            Location = dto.Location,
            Status = "Open",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Shifts.Add(shift);
        await _context.SaveChangesAsync();

        shift.Venue = await _context.Users.FindAsync(CurrentUserId);
        return CreatedAtAction(nameof(GetShift), new { id = shift.Id }, ToDto(shift));
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Venue")]
    public async Task<IActionResult> UpdateShift(int id, [FromBody] UpdateShiftDto dto)
    {
        var shift = await _context.Shifts
            .Include(s => s.Venue)
            .Include(s => s.AcceptedByUser)
            .Include(s => s.Requests).ThenInclude(r => r.Bartender)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (shift == null)
        {
            return NotFound(new { message = "Shift not found" });
        }

        if (shift.VenueId != CurrentUserId)
        {
            return Forbid();
        }

        if (dto.Title != null) shift.Title = dto.Title;
        if (dto.Description != null) shift.Description = dto.Description;
        if (dto.ShiftDate.HasValue) shift.ShiftDate = dto.ShiftDate.Value;
        if (dto.StartTime.HasValue) shift.StartTime = dto.StartTime.Value;
        if (dto.EndTime.HasValue) shift.EndTime = dto.EndTime.Value;
        if (dto.HourlyRate.HasValue) shift.HourlyRate = dto.HourlyRate.Value;
        if (dto.Location != null) shift.Location = dto.Location;
        shift.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(ToDto(shift));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Venue")]
    public async Task<IActionResult> CancelShift(int id)
    {
        var shift = await _context.Shifts.FindAsync(id);
        if (shift == null)
        {
            return NotFound(new { message = "Shift not found" });
        }

        if (shift.VenueId != CurrentUserId)
        {
            return Forbid();
        }

        shift.Status = "Cancelled";
        shift.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new { success = true, message = "Shift cancelled successfully" });
    }

    [HttpPost("{id}/request")]
    [Authorize(Roles = "Bartender")]
    public async Task<IActionResult> RequestShift(int id)
    {
        var shift = await _context.Shifts
            .Include(s => s.Venue)
            .Include(s => s.AcceptedByUser)
            .Include(s => s.Requests).ThenInclude(r => r.Bartender)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (shift == null)
        {
            return NotFound(new { message = "Shift not found" });
        }

        if (shift.Status != "Open")
        {
            return Conflict(new { message = "Shift is not open for requests" });
        }

        if (shift.Requests!.Any(r => r.BartenderId == CurrentUserId))
        {
            return Conflict(new { message = "You have already requested this shift" });
        }

        var request = new ShiftRequest
        {
            ShiftId = id,
            BartenderId = CurrentUserId,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.ShiftRequests.Add(request);
        await _context.SaveChangesAsync();

        shift.Requests!.Add(request);
        return Ok(new { success = true, shift = ToDto(shift) });
    }

    [HttpPost("{id}/accept")]
    [Authorize(Roles = "Venue")]
    public async Task<IActionResult> AcceptRequest(int id, [FromBody] ShiftRequestActionDto dto)
    {
        var shift = await _context.Shifts
            .Include(s => s.Venue)
            .Include(s => s.AcceptedByUser)
            .Include(s => s.Requests).ThenInclude(r => r.Bartender)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (shift == null)
        {
            return NotFound(new { message = "Shift not found" });
        }

        if (shift.VenueId != CurrentUserId)
        {
            return Forbid();
        }

        var request = shift.Requests!.FirstOrDefault(r => r.BartenderId == dto.BartenderId);
        if (request == null)
        {
            return NotFound(new { message = "No request from this bartender for this shift" });
        }

        request.Status = "Accepted";
        request.UpdatedAt = DateTime.UtcNow;

        foreach (var other in shift.Requests!.Where(r => r.BartenderId != dto.BartenderId && r.Status == "Pending"))
        {
            other.Status = "Rejected";
            other.UpdatedAt = DateTime.UtcNow;
        }

        shift.AcceptedByUserId = dto.BartenderId;
        shift.Status = "Filled";
        shift.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { success = true, shift = ToDto(shift) });
    }

    [HttpPost("{id}/reject")]
    [Authorize(Roles = "Venue")]
    public async Task<IActionResult> RejectRequest(int id, [FromBody] ShiftRequestActionDto dto)
    {
        var shift = await _context.Shifts
            .Include(s => s.Venue)
            .Include(s => s.AcceptedByUser)
            .Include(s => s.Requests).ThenInclude(r => r.Bartender)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (shift == null)
        {
            return NotFound(new { message = "Shift not found" });
        }

        if (shift.VenueId != CurrentUserId)
        {
            return Forbid();
        }

        var request = shift.Requests!.FirstOrDefault(r => r.BartenderId == dto.BartenderId);
        if (request == null)
        {
            return NotFound(new { message = "No request from this bartender for this shift" });
        }

        request.Status = "Rejected";
        request.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new { success = true, shift = ToDto(shift) });
    }

    private static ShiftDto ToDto(Shift shift)
    {
        return new ShiftDto
        {
            Id = shift.Id,
            VenueId = shift.VenueId,
            VenueName = shift.Venue != null ? $"{shift.Venue.FirstName} {shift.Venue.LastName}".Trim() : null,
            Title = shift.Title,
            Description = shift.Description,
            ShiftDate = shift.ShiftDate,
            StartTime = shift.StartTime,
            EndTime = shift.EndTime,
            HourlyRate = shift.HourlyRate,
            Location = shift.Location,
            Status = shift.Status,
            RequestedBy = shift.Requests?.Select(r => r.BartenderId).ToList() ?? new List<string>(),
            Requests = shift.Requests?.Select(r => new ShiftRequestSummaryDto
            {
                BartenderId = r.BartenderId,
                BartenderName = r.Bartender != null ? $"{r.Bartender.FirstName} {r.Bartender.LastName}".Trim() : null,
                Status = r.Status
            }).ToList() ?? new List<ShiftRequestSummaryDto>(),
            AcceptedBy = shift.AcceptedByUserId,
            AcceptedByName = shift.AcceptedByUser != null
                ? $"{shift.AcceptedByUser.FirstName} {shift.AcceptedByUser.LastName}".Trim()
                : null,
            CreatedAt = shift.CreatedAt,
            UpdatedAt = shift.UpdatedAt
        };
    }
}
