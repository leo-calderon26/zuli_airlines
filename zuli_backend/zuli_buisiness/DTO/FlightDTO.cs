using System;

namespace zuli_Business.DTO
{
	public class FlightDTO
	{
		public Guid flightId { get; set; }
		public string status { get; set; }
		public DateTime flightDate { get; set; }
		public decimal touristPrice { get; set; }
		public decimal firstClassPrice { get; set; }
		public DateTime realDepartureTime { get; set; }
		public DateTime realArrivalTime { get; set; }
		public DateTime checkInStartTime { get; set; }
		public DateTime checkInDeadline { get; set; }
		public int duration { get; set; }
		public decimal carryOnPrice { get; set; }
		public decimal checkedPrice { get; set; }
		public int availableSeats { get; set; }
		public int airlineId { get; set; }
		public int aircraftId { get; set; }
		public int itineraryId { get; set; }
		public int adminId { get; set; }
		public int flightRouteId { get; set; }
	}
}
