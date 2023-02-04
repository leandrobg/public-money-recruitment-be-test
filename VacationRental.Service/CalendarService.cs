using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VacationRental.Domain.Interfaces;
using VacationRental.Domain.Models.Calendar;
using VacationRental.Infra.Data.Context;

namespace VacationRental.Service
{
    public class CalendarService : ICalendarService
    {
        private readonly DataContext _context;

        public CalendarService(DataContext context)
        {
            _context = context;
        }

        public async Task<GetCalendarResponse> GetRentalCalendar(int rentalId, DateTime start, int nights)
        {
            var result = new GetCalendarResponse()
            {
                RentalId = 0,
                Dates = new List<CalendarDateModel>(),
                Message = "Rental not found"

            };
            var rentals = await _context.Rentals.Where(w => w.Id == rentalId).FirstAsync();
            var bookings = await _context.Bookings.Where(w => w.RentalId == rentalId).ToListAsync();

            if (rentals == null)
                return result;

            //var calendaryViewModel = CommonHelper.SetCalendarInstanceForRentalId(rentalId);
            var rentalUnit = rentals.Units;

            for (var i = 0; i < nights; i++)
            {
                var date = GetCalendarFromStartDate(start, i);

                foreach (var booking in bookings)
                {
                    if (booking.RentalId == rentalId
                        && booking.Start <= date.Date && booking.Start.AddDays(booking.Nights) > date.Date)
                    {
                        date.Bookings.Add(new CalendarBookingModel { Id = booking.Id, Unit = rentalUnit });
                    }
                }
                result.Dates.Add(date);
            }

            // Set preparation time for rentals
            var preparationTimeInDays = rentals.PreparationTimeInDays;
            if (preparationTimeInDays > 0)
            {
                for (var i = 0; i < preparationTimeInDays; i++)
                {
                    var addedDays = i + nights;
                    var date = GetCalendarFromStartDate(start, addedDays);
                    date.PreparationTimes.Add(new CalendarRentalUnitModel { Unit = rentalUnit });
                    result.Dates.Add(date);
                }
            }
            
            result.RentalId = rentalId;
            result.Message = "";
            return result;
        }

        private CalendarDateModel GetCalendarFromStartDate(DateTime startDate, int days)
        {
            return new CalendarDateModel
            {
                Date = startDate.Date.AddDays(days),
                Bookings = new List<CalendarBookingModel>(),
                PreparationTimes = new List<CalendarRentalUnitModel>()
            };
        }
    }
}
