using AttendanceRecorder.FileSystemStorage;

namespace AttendanceRecorder.WebApi.WorkingDay;

public class IntervalCreator(LifeSignConfig config)
{
    private static readonly TimeOnly StartOfDay = new(0, 0, 0);
    private static readonly TimeOnly EndOfDay = new(23, 59, 59);
    private readonly TimeSpan _maxBreak = 2 * config.UpdatePeriod;

    public IEnumerable<IntervalDto> CreateActiveIntervals(IEnumerable<TimeOnly> lifeSigns)
    {
        return lifeSigns
            .OrderBy(ls => ls)
            .Aggregate(
                new Stack<IntervalDto>(),
                (stack, lifeSign) =>
                {
                    var start = lifeSign >= StartOfDay.Add(_maxBreak) ? lifeSign : StartOfDay;
                    var end = lifeSign <= EndOfDay.Add(-_maxBreak) ? lifeSign : EndOfDay;

                    if (stack.Count == 0)
                    {
                        // First life sign of the day
                        stack.Push(new IntervalDto { IsActive = true, Start = start, End = end });
                    }
                    else
                    {
                        // Subsequent life signs
                        if (start <= stack.Peek().End.Add(_maxBreak))
                        {
                            // Extend the current active interval
                            var current = stack.Pop();
                            stack.Push(new IntervalDto { IsActive = true, Start = current.Start, End = end });
                        }
                        else
                        {
                            // Start a new active interval
                            stack.Push(new IntervalDto { IsActive = true, Start = start, End = end });
                        }
                    }

                    return stack;
                }).Reverse().ToList();
    }

    public IEnumerable<IntervalDto> CreateInactiveIntervals(IEnumerable<IntervalDto> activeIntervals)
    {
        var intervals = activeIntervals
            .OrderBy(i => i.Start)
            .Aggregate(
                new Stack<IntervalDto>(),
                (stack, activeInterval) =>
                {
                    if (stack.Count == 0)
                    {
                        // First interval of the day
                        if (activeInterval.Start > StartOfDay)
                        {
                            // Start the day with an inactive interval
                            stack.Push(new IntervalDto
                            {
                                IsActive = false,
                                Start = StartOfDay,
                                End = activeInterval.Start.Add(TimeSpan.FromSeconds(-1)),
                            });
                        }
                    }
                    else
                    {
                        // Add inactive interval before each active interval
                        stack.Push(new IntervalDto
                        {
                            IsActive = false,
                            Start = stack.Peek().End.Add(TimeSpan.FromSeconds(1)),
                            End = activeInterval.Start.Add(TimeSpan.FromSeconds(-1)),
                        });
                    }

                    stack.Push(activeInterval);

                    return stack;
                });

        if (intervals.Count == 0)
        {
            // Single inactive interval for the whole day
            intervals.Push(new IntervalDto { IsActive = false, Start = StartOfDay, End = EndOfDay });
        }
        else
        {
            if (intervals.Peek().End < EndOfDay)
            {
                // Add last inactive interval to the end of the day
                intervals.Push(new IntervalDto
                {
                    IsActive = false, Start = intervals.Peek().End.Add(TimeSpan.FromSeconds(1)), End = EndOfDay,
                });
            }
        }

        return intervals.Reverse().ToList();
    }
}