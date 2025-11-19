using System.Globalization;
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
        [FromRoute] string day, // DateOnly cannot be generated with NSwag in the typescript client
        [FromQuery] TimeOnly start,
        [FromQuery] TimeOnly end)
    {
        var parsedDay = DateOnly.Parse(day, CultureInfo.InvariantCulture);

        writerService.WriteInactiveMerge(parsedDay, start, end);

        var workingDay = workingDayService.Build(parsedDay);

        return Ok(workingDay);
    }
}