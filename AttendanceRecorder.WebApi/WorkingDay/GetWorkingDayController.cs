using Microsoft.AspNetCore.Mvc;

namespace AttendanceRecorder.WebApi.WorkingDay;

[ApiController]
[Route("api/working-days/{date}")]
public class GetWorkingDayController(WorkingDayService workingDayService) : ControllerBase
{
    [HttpGet(Name = nameof(GetWorkingDay))]
    public ActionResult<WorkingDayDto> GetWorkingDay([FromRoute] DateOnly date)
    {
        var workingDay = workingDayService.Build(date);

        return Ok(workingDay);
    }
}