using AttendanceRecorder.FileSystemStorage;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceRecorder.WebApi;

[ApiController]
[Route("api/years/{year:int}/weeks/{week:int}")]
public class GetDaysController(LifeSignReaderService readerService) : ControllerBase
{
    [HttpGet(Name = nameof(GetDays))]
    public ActionResult<IEnumerable<DateOnly>> GetDays([FromRoute] int year, [FromRoute] int week)
    {
        var days = readerService.GetDaysByWeek(year, week).OrderByDescending(day => day);
        return Ok(days);
    }
}