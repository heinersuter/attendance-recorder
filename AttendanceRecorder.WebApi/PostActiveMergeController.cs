using AttendanceRecorder.FileSystemStorage;
using AttendanceRecorder.WebApi.WorkingDay;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceRecorder.WebApi;

[ApiController]
[Route("api/working-days/{day}/merges/active")]
public class PostActiveMergeController(WorkingDayService workingDayService, MergeWriterService writerService)
    : ControllerBase
{
    [HttpPost(Name = nameof(PostActiveMerge))]
    public ActionResult<WorkingDayDto> PostActiveMerge(
        [FromRoute] DateOnly day,
        [FromQuery] TimeOnly start,
        [FromQuery] TimeOnly end)
    {
        writerService.WriteActiveMerge(day, start, end);

        var workingDay = workingDayService.Build(day);

        return Ok(workingDay);
    }
}