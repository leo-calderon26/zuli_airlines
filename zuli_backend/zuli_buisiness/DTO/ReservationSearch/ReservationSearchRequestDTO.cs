using System.ComponentModel.DataAnnotations;

namespace zuli_Business.DTO.ReservationSearch
{
    public class ReservationSearchRequestDTO
    {
        public const int RESERVATION_CODE_LENGTH = 8;
        public string ReservationCode { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}