using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VacationRental.Domain.Entities;
using VacationRental.Domain.Interfaces;
using VacationRental.Domain.Models.Bookings;
using VacationRental.Infra.Data.Context;

namespace VacationRental.Service
{
    public class BookingService : IBookingService
    {
        private readonly DataContext _context;
        public BookingService(DataContext context)
        {
            _context = context;
        }

        public async Task<GetBookingResponse> GetById(int bookingId)
        {
            GetBookingResponse result = new();
            var booking = await _context.Bookings.Where(w => w.Id == bookingId).FirstOrDefaultAsync();
            if (booking != null)
            {
                result = new()
                {
                    Id = booking.Id,
                    Nights = booking.Nights,
                    Start = booking.Start,
                    RentalId = booking.RentalId
                };
            }
            return result;
        }

        public async Task<List<GetBookingResponse>> GetAll()
        {
            List<GetBookingResponse> result = new();
            var bookings = await _context.Bookings.ToListAsync();
            result = bookings.Select(s => new GetBookingResponse
            {
                Id = s.Id,
                Nights = s.Nights,
                RentalId = s.RentalId,
                Start = s.Start
            }).ToList();
            return result;
        }

        public async Task<CreateBookingResponse> CreateBooking(CreateBookingRequest booking)
        {
            CreateBookingResponse result = new()
            {
                Id = 0,
                Message = "Rental units not available"
            };
            var actualRentals = await _context.Rentals.ToListAsync();
            var actualBookings = await GetAll();

            var freeUnits = 0;
            for (int i = 0; i < booking.Nights; i++)
            {
                var bookedUnits = 0;
                foreach (var item in actualBookings)
                {
                    if (CheckOccupancyAvailability(item, booking, actualRentals.Where(w => w.Id == booking.RentalId).First().PreparationTimeInDays))
                        bookedUnits++;
                    else
                        freeUnits++;
                }
                if (bookedUnits >= actualRentals.Where(w => w.Id == booking.RentalId).First().Units)
                {
                    result.Id = booking.RentalId;
                    return result;
                }
                else
                {
                    var addRental = new Booking()
                    {
                        RentalId = booking.RentalId,
                        Nights = booking.Nights,
                        Start = booking.Start

                    };

                    await _context.Bookings.AddAsync(addRental);
                    _context.SaveChanges();

                    result.Id = addRental.Id;
                    result.Message = "";
                }
            }
            return result;
        }

        public async Task<List<Booking>> GetByRentalId(int rentalId)
        {
            List<Booking> result = new();
            var bookings = await _context.Bookings.Where(w => w.RentalId == rentalId).ToListAsync();
            if (bookings != null)
                result = bookings;

            return result;
        }

        private bool CheckOccupancyAvailability(GetBookingResponse booking, CreateBookingRequest newBooking, int preparationDate)
        {
            int result = 0;
            var occupiedDays = newBooking.Nights + preparationDate;
            var currentBooking = booking.Nights + preparationDate - 1;

            if (booking.Start <= newBooking.Start.Date && 
                booking.Start.AddDays(currentBooking) > newBooking.Start.Date)
            {
                result++;
            }
            else if (booking.Start < newBooking.Start.AddDays(occupiedDays) && 
                     booking.Start.AddDays(currentBooking) >= newBooking.Start.AddDays(occupiedDays))
            {

            }
            else if (booking.Start > newBooking.Start && 
                     booking.Start.AddDays(currentBooking) < newBooking.Start.AddDays(occupiedDays))
            {

            }



            return booking.Start <= newBooking.Start.Date && booking.Start.AddDays(currentBooking) > newBooking.Start.Date
                    || (booking.Start < newBooking.Start.AddDays(occupiedDays) && booking.Start.AddDays(currentBooking) >= newBooking.Start.AddDays(occupiedDays))
                    || (booking.Start > newBooking.Start && booking.Start.AddDays(currentBooking) < newBooking.Start.AddDays(occupiedDays));
        }
    }
}
