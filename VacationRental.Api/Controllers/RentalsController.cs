using Microsoft.AspNetCore.Mvc;
using VacationRental.Domain.Interfaces;
using VacationRental.Domain.Models.Rentals;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/rentals")]
    public class RentalsController : ControllerBase
    {
        private IRentalService _rentalService;

        public RentalsController(IRentalService rentalService)
        {
            _rentalService = rentalService;
        }

        [HttpGet]
        [Route("{rentalId:int}")]
        public IActionResult Get(int rentalId)
        {
            var rental = _rentalService.GetById(rentalId).Result;
            if (rental != null)
                return Ok(rental);
            else
                return NoContent();
        }

        [HttpPost]
        public IActionResult Post(RentalRequest model)
        {
            var result = _rentalService.Create(model).Result;
            if (result != null)
            {
                return Ok(result);
            }
            else
                return NoContent();
        }

        [HttpPut]
        [Route("{rentalId:int}")]
        public IActionResult Put(int rentalId, RentalRequest model)
        {
            UpdateRentalRequest updateModel = new()
            {
                Id = rentalId,
                PreparationTimeInDays = model.PreparationTimeInDays,
                Units = model.Units
            };
            var result = _rentalService.Update(updateModel);

            if (result != null)
                return Ok(result);
            else
                return NoContent();
        }

    }
}
