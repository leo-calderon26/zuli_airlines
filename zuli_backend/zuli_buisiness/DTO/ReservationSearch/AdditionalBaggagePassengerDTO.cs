namespace zuli_Business.DTO.ReservationSearch
{
    public class AdditionalBaggagePassengerDTO
    {
        public int PassengerId { get; set; }
        public int AdditionalCheckedBaggage { get; set; }
        public int AdditionalCarryOn { get; set; }
    }
    public class AdditionalBaggageRequestDTO
    {
        public string ReservationCode { get; set; } = string.Empty;
        public List<AdditionalBaggagePassengerDTO> Passengers { get; set; } = new();
    }
}
