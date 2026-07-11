namespace GigBartending.Api.DTOs;

public static class ShiftValidation
{
    public static List<string> Validate(DateTime shiftDate, TimeSpan startTime, TimeSpan endTime, decimal hourlyRate)
    {
        var errors = new List<string>();

        if (startTime >= endTime)
        {
            errors.Add("Start time must be before end time.");
        }

        if (hourlyRate <= 0)
        {
            errors.Add("Hourly rate must be greater than zero.");
        }

        if (shiftDate.Date < DateTime.UtcNow.Date)
        {
            errors.Add("Shift date cannot be in the past.");
        }

        return errors;
    }
}
