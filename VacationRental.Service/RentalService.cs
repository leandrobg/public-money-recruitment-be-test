using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using VacationRental.Domain.Entities;
using VacationRental.Domain.Interfaces;
using VacationRental.Domain.Models.Rentals;
using VacationRental.Infra.Data.Context;

namespace VacationRental.Service
{
    public class RentalService : IRentalService
    {
        private DataContext _context;

        public RentalService(DataContext context)
        {
            _context = context;
        }
        public async Task<GetRentalResponse> GetById(int id)
        {
            var result = new GetRentalResponse();
            var rental = await _context.Rentals.Where(q => q.Id == id).FirstOrDefaultAsync();
            if (rental != null)
            {
                result = new GetRentalResponse()
                {
                    Id = rental.Id,
                    Units = rental.Units,
                    PreparationTimeInDays = rental.PreparationTimeInDays
                };

            }

            return result;
        }
        public async Task<List<GetRentalResponse>> GetAll()
        {
            var result = new List<GetRentalResponse>();
            var rentals = await _context.Rentals.ToListAsync();

            result = rentals.Select(x => new GetRentalResponse { 
                Id = x.Id,
                PreparationTimeInDays= x.PreparationTimeInDays,
                Units = x.Units
            }).ToList();
            return result;
        }
        public async Task<CreateRentalResponse> Create(RentalRequest model)
        {
            var rental = new Rental()
            {
                PreparationTimeInDays = model.PreparationTimeInDays
            };

            await _context.Rentals.AddAsync(rental);
            _context.SaveChanges();
            CreateRentalResponse result = new()
            {
                Id = rental.Id,
                Message = "Rental not found"
            };

            return result;
        }

        public async Task<CreateRentalResponse> Update(UpdateRentalRequest model)
        {
            var actualBookings = await _context.Bookings.Where(w => w.RentalId == model.Id).ToListAsync();
            var rental = await _context.Rentals.Where(q => q.Id == model.Id).FirstOrDefaultAsync();
            string message = "Rental not found";
            CreateRentalResponse result = new()
            {
                Id = model.Id
            };
            if (rental != null)
            {
                var hasConflictBookings = CheckBookedDates(actualBookings, rental, model);

                if (hasConflictBookings)
                    message = "Date not available";
                else
                {
                    var updatedRental = new Rental()
                    {
                        Id = model.Id,
                        PreparationTimeInDays = model.PreparationTimeInDays,
                        Units = model.Units,
                    };
                    _context.Rentals.Update(updatedRental);
                    _context.SaveChanges();
                }
            }
            result.Message = message;
            return result;
        }

        private bool CheckBookedDates(List<Booking> bookings, Rental rental, UpdateRentalRequest rentalBindingModel)
        {
            foreach (var booking in bookings)
            {
                var newTotalBookedDays = booking.Start.AddDays(booking.Nights + rentalBindingModel.PreparationTimeInDays);
                var currentTotalBookedDays = booking.Start.AddDays(booking.Nights + rental.PreparationTimeInDays);
                if (newTotalBookedDays > currentTotalBookedDays)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
