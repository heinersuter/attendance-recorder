using AttendanceRecorder.FileSystemStorage;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceRecorder.WebApi;

[ApiController]
[Route("api/years")]
public class GetYearsController(LifeSignReaderService lifeSignReaderService) : ControllerBase
{
    [HttpGet(Name = nameof(GetYears))]
    public ActionResult<IEnumerable<int>> GetYears()
    {
        var years = lifeSignReaderService.GetYears().OrderByDescending(year => year);
        return Ok(years);
    }
}