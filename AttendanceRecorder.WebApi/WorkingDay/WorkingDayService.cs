using AttendanceRecorder.FileSystemStorage;
using Microsoft.Extensions.Options;

namespace AttendanceRecorder.WebApi.WorkingDay;

public class WorkingDayService(IOptions<LifeSignConfig> config)
{
    private static readonly TimeOnly StartOfDay = new TimeOnly(0, 0, 0);
    private static readonly TimeOnly EndOfDay = new TimeOnly(23, 59, 59);
    private readonly TimeSpan _maxBreak = 2 * config.Value.UpdatePeriod;

    public WorkingDayDto Build(DateOnly date, IOrderedEnumerable<TimeOnly> lifeSigns)
    {
        return new WorkingDayDto { Date = date, Intervals = BuildIntervals(new Queue<TimeOnly>(lifeSigns)) };
    }

    private List<IntervalDto> BuildIntervals(IEnumerable<TimeOnly> lifeSigns)
    {
        var activeIntervals = lifeSigns.Aggregate(new Stack<IntervalDto>(), (stack, lifeSign) =>
        {
            if (stack.Count == 0)
            {
                if (StartOfDay.Add(_maxBreak) >= lifeSign)
                {
                    stack.Push(new IntervalDto { IsActive = true, Start = StartOfDay, End = lifeSign });
                }
                else
                {
                    stack.Push(new IntervalDto { IsActive = true, Start = lifeSign, End = lifeSign });
                }
            }
            else
            {
                if (stack.Peek().End.Add(_maxBreak) >= lifeSign)
                {
                    var current = stack.Pop();
                    stack.Push(new IntervalDto { IsActive = true, Start = current.Start, End = lifeSign });
                }
                else
                {
                    stack.Push(new IntervalDto { IsActive = true, Start = lifeSign, End = lifeSign });
                }
            }

            return stack;
        }).Reverse().ToList();

        var intervals = activeIntervals.Aggregate(new Stack<IntervalDto>(), (stack, interval) =>
        {
            if (stack.Count == 0)
            {
                if (StartOfDay.Add(_maxBreak) < interval.Start)
                {
                    stack.Push(new IntervalDto
                    {
                        IsActive = false, Start = StartOfDay, End = interval.Start.Add(TimeSpan.FromSeconds(-1)),
                    });
                    stack.Push(interval);
                }
                else
                {
                    stack.Push(new IntervalDto { IsActive = true, Start = StartOfDay, End = interval.End, });
                }
            }
            else
            {
                stack.Push(new IntervalDto
                {
                    IsActive = false,
                    Start = stack.Peek().End.Add(TimeSpan.FromSeconds(1)),
                    End = interval.Start.Add(TimeSpan.FromSeconds(-1)),
                });
                stack.Push(interval);
            }

            return stack;
        });

        if (intervals.Count == 0)
        {
            intervals.Push(new IntervalDto { IsActive = false, Start = StartOfDay, End = EndOfDay });
        }
        else
        {
            if (intervals.Peek().End >= EndOfDay.Add(-_maxBreak))
            {
                var lastInterval = intervals.Pop();
                intervals.Push(new IntervalDto { IsActive = true, Start = lastInterval.Start, End = EndOfDay, });
            }
            else
            {
                intervals.Push(new IntervalDto
                {
                    IsActive = false, Start = intervals.Peek().End.Add(TimeSpan.FromSeconds(1)), End = EndOfDay,
                });
            }
        }

        return intervals.Reverse().ToList();
    }
}