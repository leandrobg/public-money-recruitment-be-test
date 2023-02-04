using VacationRental.Domain.Entities;
using VacationRental.Domain.Models.Rentals;

namespace VacationRental.Domain.Interfaces
{
    public interface IRentalService
    {
        Task<GetRentalResponse> GetById(int id);
        Task<List<GetRentalResponse>> GetAll();
        Task<CreateRentalResponse> Create(RentalRequest model);
        Task<CreateRentalResponse> Update(UpdateRentalRequest model);
    }
}
