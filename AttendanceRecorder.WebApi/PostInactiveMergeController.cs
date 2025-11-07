using AttendanceRecorder.FileSystemStorage;
using AttendanceRecorder.WebApi.WorkingDay;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceRecorder.WebApi;

[ApiController]
[Route("api/working-days/{date}/merges/inactive")]
public class PostInactiveMergeController(WorkingDayService workingDayService, MergeWriterService writerService)
    : ControllerBase
{
    [HttpPost(Name = nameof(PostInactiveMerge))]
    public ActionResult<WorkingDayDto> PostInactiveMerge(
        [FromRoute] DateOnly date,
        [FromQuery] TimeOnly start,
        [FromQuery] TimeOnly end)
    {
        writerService.WriteInactiveMerge(date, start, end);

        var workingDay = workingDayService.Build(date);

        return Ok(workingDay);
    }
}