using System.ComponentModel.DataAnnotations;

namespace AttendanceRecorder.WebApi.WorkingDay;

public class WorkingDayDto
{
    public required DateOnly Date { get; init; }

    [Required]
    public required IEnumerable<IntervalDto> Intervals { get; init; } = [];

    public TimeSpan ActiveDuration => Intervals
        .Where(i => i.IsActive)
        .Aggregate(
            TimeSpan.Zero,
            (sum, interval) => sum + interval.Duration);
}