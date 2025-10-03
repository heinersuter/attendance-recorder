using AttendanceRecorder.FileSystemStorage;
using Microsoft.Extensions.Options;

namespace AttendanceRecorder.WebApi.Model;

public class WorkingDayService(IOptions<LifeSignConfig> config)
{
    private readonly TimeSpan _maxBreak = 2 * config.Value.UpdatePeriod;

    public WorkingDay Build(DateOnly date, IOrderedEnumerable<TimeOnly> lifeSigns)
    {
        return new WorkingDay { Date = date, Intervals = BuildIntervals(new Queue<TimeOnly>(lifeSigns)) };
    }

    private List<Interval> BuildIntervals(Queue<TimeOnly> lifeSigns)
    {
        var intervals = new List<Interval>();
        var currentStart = new TimeOnly(0, 0, 0);
        var currentEnd = new TimeOnly(0, 0, 0);

        while (lifeSigns.Count > 0)
        {
            var currentLifeSign = lifeSigns.Dequeue();

            if (currentEnd.Add(_maxBreak) > currentLifeSign)
            {
                // Extend current interval
                currentEnd = currentLifeSign;
            }
            else
            {
                if (currentStart != currentEnd)
                {
                    // Add current active interval
                    intervals.Add(new Interval { IsActive = true, Start = currentStart, End = currentEnd, });
                }

                // Add inactive interval
                intervals.Add(new Interval
                {
                    IsActive = false,
                    Start = currentEnd.Add(TimeSpan.FromSeconds(1)),
                    End = currentLifeSign.Add(TimeSpan.FromSeconds(-1)),
                });

                currentStart = currentLifeSign;
                currentEnd = currentLifeSign;
            }
        }

        if (currentStart != currentEnd)
        {
            // Add last active interval
            intervals.Add(new Interval { IsActive = true, Start = currentStart, End = currentEnd, });
        }

        // Add last inactive interval until 23:59:59
        var endOfDay = new TimeOnly(23, 59, 59);
        if (currentEnd < endOfDay)
        {
            intervals.Add(new Interval
            {
                IsActive = false,
                Start = currentEnd.Add(TimeSpan.FromSeconds(1)),
                End = endOfDay,
            });
        }

        return intervals;
    }
}