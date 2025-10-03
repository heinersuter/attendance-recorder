using AttendanceRecorder.FileSystemStorage;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceRecorder.WebApi;

[ApiController]
[Route("api/years/{year:int}/dates")]
public class GetDatesController(LifeSignReaderService readerService) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<string>> GetDates([FromRoute] int year)
    {
        var dates = readerService.GetDates(year);
        if (!dates.Any())
        {
            return NotFound($"No dates found for year '{year}'.");
        }

        return Ok(dates);
    }
}