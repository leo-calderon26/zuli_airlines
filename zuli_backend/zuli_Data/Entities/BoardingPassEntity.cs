using System;

namespace zuli_Data.Entities
{
    public class BoardingPassEntity
    {
        public Guid FlightId { get; set; }
        public string ReservationCode { get; set; } = string.Empty;
        public int PassengerId { get; set; }
    }
}
