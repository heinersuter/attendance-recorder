using AttendanceRecorder.FileSystemStorage;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceRecorder.WebApi
{
    [ApiController]
    [Route("api/years/{year:int}/weeks")]
    public class GetWeeksController(LifeSignReaderService readerService) : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<int>> GetWeeks([FromRoute] int year)
        {
            var weeks = readerService.GetWeeksByYear(year);

            return Ok(weeks);
        }
    }
}