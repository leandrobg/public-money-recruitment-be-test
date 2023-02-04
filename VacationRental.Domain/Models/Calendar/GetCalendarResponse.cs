using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VacationRental.Domain.Models.Calendar
{
    public class GetCalendarResponse
    {
        public int RentalId { get; set; }
        public List<CalendarDateModel> Dates { get; set; }

        public string Message { get; set; }
    }
}
