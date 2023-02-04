using Microsoft.AspNetCore.Mvc;
using VacationRental.Domain.Interfaces;
using VacationRental.Domain.Models.Calendar;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/calendar")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly ICalendarService _calendarService;

        public CalendarController(ICalendarService calendarService)
        {
            _calendarService = calendarService;
        }

        [HttpGet]
        public IActionResult Get(GetCalendarRequest model)
        {
            var result = _calendarService.GetRentalCalendar(model.RentalId, model.Start, model.Nights).Result;
            return Ok(result);
        }
    }
}
