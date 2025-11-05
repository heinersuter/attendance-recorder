using AttendanceRecorder.FileSystemStorage;

namespace AttendanceRecorder.WebApi.WorkingDay;

public class IntervalsMerger(LifeSignConfig config)
{
    private readonly TimeSpan _maxBreak = 2 * config.UpdatePeriod;

    public IEnumerable<IntervalDto> MergeActiveIntervals(
        IEnumerable<IntervalDto> activeIntervals,
        IEnumerable<(TimeOnly Start, TimeOnly End)> activeMerges)
    {
        return activeIntervals
            .Concat(activeMerges.Select(merge =>
                new IntervalDto { Start = merge.Start, End = merge.End, IsActive = true }))
            .OrderBy(i => i.Start)
            .Aggregate(
                new List<IntervalDto>(),
                (intervals, current) =>
                {
                    if (intervals.Count == 0)
                    {
                        // Add initial interval
                        intervals.Add(current);
                    }
                    else
                    {
                        var last = intervals[^1];
                        if (current.Start <= last.End.Add(_maxBreak))
                        {
                            // Merge with the last interval
                            intervals[^1] = new IntervalDto
                            {
                                Start = last.Start,
                                End = current.End > last.End ? current.End : last.End,
                                IsActive = true,
                            };
                        }
                        else
                        {
                            // Add new interval
                            intervals.Add(current);
                        }
                    }

                    return intervals;
                }).ToList();
    }

    public IEnumerable<IntervalDto> MergeInactiveIntervals(
        IEnumerable<IntervalDto> activeIntervals,
        IEnumerable<(TimeOnly Start, TimeOnly End)> inactiveMerges)
    {
        var intervals = activeIntervals.ToList();

        foreach (var merge in inactiveMerges)
        {
            var intervalsToRemove = new List<IntervalDto>();
            var intervalsToAdd = new List<IntervalDto>();

            foreach (var interval in intervals)
            {
                if (merge.Start <= interval.Start && merge.End >= interval.End)
                {
                    // Active interval completely covered by inactive merge
                    intervalsToRemove.Add(interval);
                    continue;
                }

                if (merge.Start > interval.Start && merge.End < interval.End)
                {
                    // Active interval spans inactive merge, split into two
                    intervalsToRemove.Add(interval);
                    intervalsToAdd.Add(new IntervalDto
                    {
                        Start = interval.Start, End = merge.Start.Add(TimeSpan.FromSeconds(-1)), IsActive = true,
                    });
                    intervalsToAdd.Add(new IntervalDto
                    {
                        Start = merge.End.Add(TimeSpan.FromSeconds(1)), End = interval.End, IsActive = true,
                    });
                    continue;
                }

                if (merge.Start <= interval.Start && merge.End > interval.Start)
                {
                    // Overlap at the start of the active interval
                    intervalsToRemove.Add(interval);
                    intervalsToAdd.Add(new IntervalDto
                    {
                        Start = merge.End.Add(TimeSpan.FromSeconds(1)), End = interval.End, IsActive = true,
                    });
                    continue;
                }

                if (merge.Start < interval.End && merge.End >= interval.End)
                {
                    // Overlap at the end of the active interval
                    intervalsToRemove.Add(interval);
                    intervalsToAdd.Add(new IntervalDto
                    {
                        Start = interval.Start, End = merge.Start.Add(TimeSpan.FromSeconds(-1)), IsActive = true,
                    });
                }
            }

            foreach (var interval in intervalsToRemove)
            {
                intervals.Remove(interval);
            }

            intervals.AddRange(intervalsToAdd);
        }

        return intervals.OrderBy(i => i.Start).ToList();
    }
}