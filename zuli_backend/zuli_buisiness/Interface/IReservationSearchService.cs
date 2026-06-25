using System.Threading.Tasks;
using zuli_Business.DTO.ReservationSearch;

namespace zuli_Business.Interface
{
    public interface IReservationSearchService
    {
        Task<ReservationSearchResponseDTO> GetReservationDetailsAsync(ReservationSearchRequestDTO request);
    }
}
