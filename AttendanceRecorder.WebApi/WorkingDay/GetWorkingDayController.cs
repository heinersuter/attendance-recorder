using System.Globalization;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceRecorder.WebApi.WorkingDay;

[ApiController]
[Route("api/working-days/{day}")]
public class GetWorkingDayController(WorkingDayService workingDayService) : ControllerBase
{
    [HttpGet(Name = nameof(GetWorkingDay))]
    public ActionResult<WorkingDayDto> GetWorkingDay(
        [FromRoute] string day) // DateOnly cannot be generated with NSwag in the typescript client
    {
        var workingDay = workingDayService.Build(DateOnly.Parse(day, CultureInfo.InvariantCulture));

        return Ok(workingDay);
    }
}