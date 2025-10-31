using AttendanceRecorder.FileSystemStorage;
using Microsoft.Extensions.Options;

namespace AttendanceRecorder.WebApi.WorkingDay;

public class WorkingDayService(
    IOptions<LifeSignConfig> config,
    LifeSignReaderService lifeSignReaderService,
    MergeReaderService mergeReaderService)
{
    private readonly IntervalCreator _intervalCreator = new(config.Value);
    private readonly IntervalsMerger _intervalsMerger = new(config.Value);

    public WorkingDayDto Build(DateOnly date)
    {
        var lifeSigns = lifeSignReaderService.GetLifeSigns(date);
        var merges = mergeReaderService.GetMerges(date);

        var activeIntervals = _intervalCreator.CreateActiveIntervals(lifeSigns);

        var mergedIntervals = _intervalsMerger.MergeIntervals(activeIntervals, merges);

        var intervals = _intervalCreator.CreateInactiveIntervals(mergedIntervals);

        return new WorkingDayDto { Date = date, Intervals = intervals.ToList() };
    }
}