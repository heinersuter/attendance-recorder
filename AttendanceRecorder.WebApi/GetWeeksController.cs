using AttendanceRecorder.FileSystemStorage;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceRecorder.WebApi
{
    [ApiController]
    [Route("api/years/{year:int}/weeks")]
    public class GetWeeksController(LifeSignReaderService readerService) : ControllerBase
    {
        [HttpGet(Name = nameof(GetWeeks))]
        public ActionResult<IEnumerable<int>> GetWeeks([FromRoute] int year)
        {
            var weeks = readerService.GetWeeksByYear(year).OrderByDescending(week => week);
            return Ok(weeks);
        }
    }
}