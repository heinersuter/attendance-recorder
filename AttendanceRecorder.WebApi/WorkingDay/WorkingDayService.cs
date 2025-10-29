using AttendanceRecorder.FileSystemStorage;
using Microsoft.Extensions.Options;

namespace AttendanceRecorder.WebApi.WorkingDay;

public class WorkingDayService(
    IOptions<LifeSignConfig> config,
    LifeSignReaderService lifeSignReaderService,
    MergeReaderService mergeReaderService)
{
    public WorkingDayDto Build(DateOnly date)
    {
        var lifeSigns = lifeSignReaderService.GetLifeSigns(date).OrderBy(ls => ls);
        var lifeSignCombiner = new LifeSignCombiner(config.Value);
        var intervals = lifeSignCombiner.CombineLifeSigns(lifeSigns);

        var merges = mergeReaderService.GetMerges(date);
        intervals = IntervalsMerger.MergeIntervals(intervals.OrderBy(i => i.Start), merges);

        return new WorkingDayDto { Date = date, Intervals = intervals.ToList() };
    }
}