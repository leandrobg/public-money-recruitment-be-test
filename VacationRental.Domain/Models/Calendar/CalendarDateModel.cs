using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VacationRental.Domain.Models.Calendar
{
    public class CalendarDateModel
    {
        public DateTime Date { get; set; }
        public List<CalendarBookingModel> Bookings { get; set; }
        public List<CalendarRentalUnitModel> PreparationTimes { get; set; }
    }
}
