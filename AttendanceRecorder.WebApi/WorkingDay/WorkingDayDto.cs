namespace AttendanceRecorder.WebApi.WorkingDay;

public class WorkingDayDto
{
    public required DateOnly Date { get; init; }

    public required IEnumerable<IntervalDto> Intervals { get; init; } = [];
}