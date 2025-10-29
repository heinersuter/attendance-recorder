namespace AttendanceRecorder.WebApi.WorkingDay;

public static class IntervalsMerger
{
    public static IEnumerable<IntervalDto> MergeIntervals(
        IOrderedEnumerable<IntervalDto> intervals,
        IEnumerable<(TimeOnly Start, TimeOnly End)> merges)
    {
        return intervals;
    }
}