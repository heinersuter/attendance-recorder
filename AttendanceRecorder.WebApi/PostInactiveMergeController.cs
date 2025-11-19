using AttendanceRecorder.FileSystemStorage;
using AttendanceRecorder.WebApi.WorkingDay;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceRecorder.WebApi;

[ApiController]
[Route("api/working-days/{day}/merges/inactive")]
public class PostInactiveMergeController(WorkingDayService workingDayService, MergeWriterService writerService)
    : ControllerBase
{
    [HttpPost(Name = nameof(PostInactiveMerge))]
    public ActionResult<WorkingDayDto> PostInactiveMerge(
        [FromRoute] DateOnly day,
        [FromQuery] TimeOnly start,
        [FromQuery] TimeOnly end)
    {
        writerService.WriteInactiveMerge(day, start, end);

        var workingDay = workingDayService.Build(day);

        return Ok(workingDay);
    }
}