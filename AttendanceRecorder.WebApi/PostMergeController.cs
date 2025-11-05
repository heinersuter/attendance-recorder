using AttendanceRecorder.FileSystemStorage;
using AttendanceRecorder.WebApi.WorkingDay;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceRecorder.WebApi;

[ApiController]
[Route("api/working-days/{date}/merges")]
public class PostMergeController(WorkingDayService workingDayService, MergeWriterService writerService) : ControllerBase
{
    [HttpPost(Name = nameof(PostMerge))]
    public ActionResult<WorkingDayDto> PostMerge(
        [FromRoute] DateOnly date,
        [FromQuery] TimeOnly start,
        [FromQuery] TimeOnly end)
    {
        writerService.WriteActiveMerge(date, start, end);

        var workingDay = workingDayService.Build(date);

        return Ok(workingDay);
    }
}