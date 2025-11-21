using System.ComponentModel.DataAnnotations;

namespace AttendanceRecorder.WebApi.WorkingDay;

public class IntervalDto
{
    [Required]
    public required bool IsActive { get; init; }

    [Required]
    public required TimeOnly Start { get; init; }

    [Required]
    public required TimeOnly End { get; init; }

    [Required]
    public TimeSpan Duration => End - Start;

    [Required]
    public int DurationPercentage => (int)double.Round(100.0 * Duration.TotalSeconds / TimeSpan.FromHours(24).TotalSeconds);
}