using zuli_Business.DTO.ReservationSearch;

namespace zuli_Business.Interface
{
    public interface IReservationItineraryPdfService
    {
        byte[] GenerateItineraryPdf(ReservationSearchResponseDTO reservation);
    }
}