namespace AttendanceRecorder.WebApi.WorkingDay;

public class IntervalDto
{
    public required bool IsActive { get; init; }

    public required TimeOnly Start { get; init; }

    public required TimeOnly End { get; init; }
}