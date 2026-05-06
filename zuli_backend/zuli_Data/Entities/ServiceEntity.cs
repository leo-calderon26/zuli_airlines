using System;

namespace zuli_Data.Entities
{
    public class ServiceEntity
    {
        public int Id { get; set; }
        public Guid FlightId { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
