using AttendanceRecorder.FileSystemStorage;

namespace AttendanceRecorder.WebApi.WorkingDay;

public class IntervalsMerger(LifeSignConfig config)
{
    private readonly TimeSpan _maxBreak = 2 * config.UpdatePeriod;

    public IEnumerable<IntervalDto> MergeIntervals(
        IEnumerable<IntervalDto> activeIntervals,
        IEnumerable<(TimeOnly Start, TimeOnly End)> merges)
    {
        return MergeIntervals(activeIntervals
            .Concat(merges.Select(merge => new IntervalDto { Start = merge.Start, End = merge.End, IsActive = true }))
            .ToList());
    }

    private IEnumerable<IntervalDto> MergeIntervals(IEnumerable<IntervalDto> activeIntervals)
    {
        var sortedIntervals = activeIntervals.OrderBy(i => i.Start).ToList();
        if (sortedIntervals.Count == 0)
        {
            yield break;
        }

        var current = sortedIntervals[0];
        for (var i = 1; i < sortedIntervals.Count; i++)
        {
            var next = sortedIntervals[i];
            if (next.Start <= current.End.Add(_maxBreak))
            {
                // Merge intervals
                current = new IntervalDto
                {
                    Start = current.Start, End = next.End > current.End ? next.End : current.End, IsActive = true,
                };
            }
            else
            {
                yield return current;
                current = next;
            }
        }

        yield return current;
    }
}