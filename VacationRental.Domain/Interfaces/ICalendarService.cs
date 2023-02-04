using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VacationRental.Domain.Models.Calendar;

namespace VacationRental.Domain.Interfaces
{
    public interface ICalendarService
    {
        Task<GetCalendarResponse> GetRentalCalendar(int rentalId, DateTime start, int nights);
    }
}
