using AttendanceRecorder.FileSystemStorage;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceRecorder.WebApi.WorkingDay;

[ApiController]
[Route("api/work-days/{date}")]
public class GetWorkingDayController(WorkingDayService workingDayService, LifeSignReaderService lifeSignReaderService)
    : ControllerBase
{
    [HttpGet(Name = nameof(GetWorkingDay))]
    public ActionResult<WorkingDayDto> GetWorkingDay([FromRoute] DateOnly date)
    {
        var lifeSigns = lifeSignReaderService.GetLifeSigns(date).OrderBy(ls => ls);
        var workingDay = workingDayService.Build(date, lifeSigns);

        return Ok(workingDay);
    }
}