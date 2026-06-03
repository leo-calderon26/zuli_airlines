using zuli_Business.DTO;
using zuli_Business.Interface;
using zuli_Data.Entities;
using zuli_Repository.Interface;

namespace zuli_Business
{
    public class ReservationCreationService : IReservationCreationService
    {
        private readonly IReservationRepository _reservationRepository;

        public ReservationCreationService(IReservationRepository reservationRepository)
        {
            _reservationRepository = reservationRepository;
        }

        public async Task<int> CreateReservation(string code, TicketPurchaseRequestDTO request, decimal total,
            int buyerId)
        {
            var reservation = new ReservationEntity
            {
                ReservationCode = code,
                ReservationOrigin = request.ReservationOrigin,
                TotalPayment = total,
                PurchaseDate = DateTime.Now.Date,
                BuyerId = buyerId,
                FlightClass = request.FlightClass,
                PaymentMethod = request.PaymentMethod
            };

            return await _reservationRepository.CreateReservation(reservation);
        }
    }
}
