using System;

namespace zuli_Business.DTO
{
	public class FlightDTO
	{
		public Guid? Id { get; set; }
		public string Status { get; set; } = string.Empty;
		public DateTime FlightDate { get; set; }
		public decimal TouristPrice { get; set; }
		public decimal FirstClassPrice { get; set; }
		public DateTime? RealDepartureTime { get; set; }
		public DateTime? RealArrivalTime { get; set; }
		public DateTime? CheckInStartTime { get; set; }
		public DateTime? CheckInDeadline { get; set; }
		public int AirlineId { get; set; }
		public Guid AircraftId { get; set; }
		public int ItineraryId { get; set; }
		public int Duration { get; set; }
		public decimal? CarryOnPrice { get; set; }
		public decimal? CheckedPrice { get; set; }
		public int AvailableSeats { get; set; }
		public Guid AdminId { get; set; }
		public int FlightRouteId { get; set; }
	}
}
