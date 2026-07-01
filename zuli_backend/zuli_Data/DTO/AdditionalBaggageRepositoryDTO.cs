namespace zuli_Data.DTO
{
    public sealed record PassengerBaggageCountDTO(int PassengerId, int Count);

    public sealed class AdditionalBaggageFlightPriceDTO
    {
        public decimal CheckedPrice { get; set; }
        public decimal CarryOnPrice { get; set; }
        public decimal CheckedBagMultiplier { get; set; }
    }

    public sealed class InsufficientBaggageFlightDTO
    {
        public Guid FlightId { get; set; }
        public decimal BaggageCapacity { get; set; }
        public decimal UsedBaggageWeight { get; set; }
    }
}
