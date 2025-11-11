namespace AttendanceRecorder.WebApi.WorkingDay;

public class IntervalDto
{
    public required bool IsActive { get; init; }

    public required TimeOnly Start { get; init; }

    public required TimeOnly End { get; init; }

    public TimeSpan Duration => End - Start;

    public int DurationPercentage => (int)double.Round(100.0 * Duration.TotalSeconds / TimeSpan.FromHours(24).TotalSeconds);
}