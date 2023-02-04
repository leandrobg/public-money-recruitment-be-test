using Microsoft.AspNetCore.Mvc;
using SQLitePCL;
using VacationRental.Domain.Interfaces;
using VacationRental.Domain.Models.Bookings;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/bookings")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private IBookingService _bookingService;
        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet("{bookingId:int}")]
        public IActionResult Get(int id)
        {
            var result = _bookingService.GetById(id).Result;
            if (result != null)
                return Ok(result);
            else
                return NoContent();
        }

        [HttpPost]
        public IActionResult Post(CreateBookingRequest model)
        {
            var result = _bookingService.CreateBooking(model).Result;
            if (result != null)
                return Ok(result);
            else
                return NoContent();
        }
    }
}
