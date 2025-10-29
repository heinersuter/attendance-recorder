using AttendanceRecorder.FileSystemStorage;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceRecorder.WebApi;

[ApiController]
[Route("api/work-days/{date}/merges")]
public class PostMergeController(MergeWriterService writerService) : ControllerBase
{
    [HttpPost(Name = nameof(PostMerge))]
    public ActionResult PostMerge([FromRoute] DateOnly date, [FromQuery] TimeOnly start, [FromQuery] TimeOnly end)
    {
        writerService.WriteMerge(date, start, end);
        return Ok();
    }
}