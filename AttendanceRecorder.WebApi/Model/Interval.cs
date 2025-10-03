namespace AttendanceRecorder.WebApi.Model;

public class Interval
{
    public required bool IsActive { get; init; }

    public required TimeOnly Start { get; init; }

    public required TimeOnly End { get; init; }
}