using AttendanceRecorder.FileSystemStorage;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceRecorder.WebApi.WorkingDay;

[ApiController]
[Route("api/work-days/{date}")]
public class GetWorkingDayController(
    WorkingDayService workingDayService)
    : ControllerBase
{
    [HttpGet(Name = nameof(GetWorkingDay))]
    public ActionResult<WorkingDayDto> GetWorkingDay([FromRoute] DateOnly date)
    {
        var workingDay = workingDayService.Build(date);

        return Ok(workingDay);
    }
}