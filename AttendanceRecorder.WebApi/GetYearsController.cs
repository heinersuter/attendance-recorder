using AttendanceRecorder.FileSystemStorage;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceRecorder.WebApi;

[ApiController]
[Route("api/years")]
public class GetYearsController(LifeSignReaderService lifeSignReaderService) : ControllerBase
{
    [HttpGet]
    public IActionResult GetYears()
    {
        var years = lifeSignReaderService.GetYears();
        return Ok(years);
    }
}