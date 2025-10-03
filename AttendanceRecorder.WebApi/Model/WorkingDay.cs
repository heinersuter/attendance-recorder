namespace AttendanceRecorder.WebApi.Model;

public class WorkingDay
{
    public required DateOnly Date { get; init; }

    public required List<Interval> Intervals { get; init; } = [];
}