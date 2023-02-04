using System.ComponentModel.DataAnnotations.Schema;

namespace VacationRental.Domain.Entities
{
    public class Booking
    {
        public int Id { get; set; }

        [ForeignKey("RentalId")]
        public int RentalId { get; set; }

        public DateTime Start { get; set; }

        public int Nights { get; set; }
    }
}
