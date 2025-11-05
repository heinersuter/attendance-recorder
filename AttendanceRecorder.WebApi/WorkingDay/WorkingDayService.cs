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
        var activeMerges = mergeReaderService.GetActiveMerges(date);
        var inactiveMerges = mergeReaderService.GetInactiveMerges(date);

        var intervals = _intervalCreator.CreateActiveIntervals(lifeSigns);

        intervals = _intervalsMerger.MergeActiveIntervals(intervals, activeMerges);

        intervals = _intervalsMerger.MergeInactiveIntervals(intervals, inactiveMerges);

        intervals = _intervalCreator.CreateInactiveIntervals(intervals);

        return new WorkingDayDto { Date = date, Intervals = intervals.ToList() };
    }
}