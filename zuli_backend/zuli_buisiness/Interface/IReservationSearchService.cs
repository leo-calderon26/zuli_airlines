using System.Threading.Tasks;
using zuli_Business.DTO.ReservationSearch;

namespace zuli_Business.Interface
{
    public interface IReservationSearchService
    {
        Task<ReservationSearchResponseDTO> GetReservationDetailsAsync(ReservationSearchRequestDTO request);
        Task<(byte[] FileContents, string ContentType, string FileName)> GenerateItineraryPdfAsync(ReservationSearchRequestDTO request);
    }
}
