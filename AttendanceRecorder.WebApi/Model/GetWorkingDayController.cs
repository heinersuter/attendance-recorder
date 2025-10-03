using AttendanceRecorder.FileSystemStorage;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceRecorder.WebApi.Model;

[ApiController]
[Route("api/work-days/{date}")]
public class GetWorkingDayController(WorkingDayService workingDayService, LifeSignReaderService lifeSignReaderService)
    : ControllerBase
{
    [HttpGet]
    public ActionResult<WorkingDay> GetWorkingDay([FromRoute] DateOnly date)
    {
        var lifeSigns = lifeSignReaderService.GetLifeSigns(date).OrderBy(ls => ls);
        var workingDay = workingDayService.Build(date, lifeSigns);

        return Ok(workingDay);
    }
}