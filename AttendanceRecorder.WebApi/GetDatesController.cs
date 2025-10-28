using AttendanceRecorder.FileSystemStorage;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceRecorder.WebApi;

[ApiController]
[Route("api/years/{year:int}/weeks/{week:int}")]
public class GetDatesController(LifeSignReaderService readerService) : ControllerBase
{
    [HttpGet(Name = nameof(GetDates))]
    public ActionResult<IEnumerable<DateOnly>> GetDates([FromRoute] int year, [FromRoute] int week)
    {
        var dates = readerService.GetDatesByWeek(year, week).OrderByDescending(date => date);
        return Ok(dates);
    }
}