using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VacationRental.Domain.Entities;
using VacationRental.Domain.Models.Bookings;

namespace VacationRental.Domain.Interfaces
{
    public interface IBookingService
    {
        Task<GetBookingResponse> GetById(int bookingId);
        Task<List<GetBookingResponse>> GetAll();
        Task<List<Booking>> GetByRentalId(int rentalId);
        Task<CreateBookingResponse> CreateBooking(CreateBookingRequest newBooking);
    }
}
