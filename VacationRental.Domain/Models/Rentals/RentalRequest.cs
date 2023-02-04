using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VacationRental.Domain.Models.Rentals
{
    public class RentalRequest
    {
        public int Units { get; set; }
        public int PreparationTimeInDays { get; set; }
    }
}
