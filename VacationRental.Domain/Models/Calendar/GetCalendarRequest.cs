using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VacationRental.Domain.Models.Calendar
{
    public class GetCalendarRequest
    {
        [Required]
        public int RentalId { get; set; }
        [Required]
        public DateTime Start { get; set; }
        [Required]
        public int Nights { get; set; }
    }
}
